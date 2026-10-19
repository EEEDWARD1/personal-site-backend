"use client";

import { useEffect, useState } from "react";

type PostTitle = { id: string; title: string };
type Page = { items: PostTitle[]; page: number; pageSize: number; totalCount: number };
type Post = PostTitle & {
  slug: string; content: string; summary: string; type: string;
  featured: boolean; publishedAt: string | null; createdAt: string; updatedAt: string;
};

const emptyPost = { slug: "", title: "", content: "", summary: "", type: "thought", featured: false, publishedAt: null };
const format = (value: unknown) => JSON.stringify(value, null, 2);

async function checked(response: Response) {
  if (response.ok) return response;
  if (response.status === 412)
    throw new Error("This post changed elsewhere. Your draft is preserved. Copy it before reloading to reconcile changes.");
  const body = await response.json().catch(() => null);
  const fields = body?.errors ? Object.entries(body.errors)
    .map(([name, errors]) => `${name}: ${Array.isArray(errors) ? errors.join(" ") : errors}`).join("\n") : "";
  throw new Error(fields || body?.title || `Request failed (${response.status}).`);
}

export default function PostWorkspace() {
  const [posts, setPosts] = useState<Page | null>(null);
  const [selected, setSelected] = useState<(PostTitle & { updatedAt?: string }) | null>(null);
  const [text, setText] = useState("");
  const [savedText, setSavedText] = useState("");
  const [etag, setEtag] = useState<string | null>(null);
  const [editing, setEditing] = useState(false);
  const [busy, setBusy] = useState(true);
  const [message, setMessage] = useState("");
  const dirty = text !== savedText;

  async function loadPage(page = 1) {
    const response = await checked(await fetch(`/api/posts?page=${page}&pageSize=20`, { cache: "no-store" }));
    setPosts(await response.json());
  }

  useEffect(() => {
    let cancelled = false;
    fetch("/api/posts?page=1&pageSize=20", { cache: "no-store" })
      .then(checked)
      .then(response => response.json())
      .then(data => { if (!cancelled) setPosts(data); })
      .catch(error => { if (!cancelled) setMessage(error.message); })
      .finally(() => { if (!cancelled) setBusy(false); });
    return () => { cancelled = true; };
  }, []);

  useEffect(() => {
    if (!dirty) return;
    const warn = (event: BeforeUnloadEvent) => { event.preventDefault(); };
    window.addEventListener("beforeunload", warn);
    return () => window.removeEventListener("beforeunload", warn);
  }, [dirty]);

  async function run(action: () => Promise<void>, success = "") {
    setBusy(true);
    setMessage("");
    try { await action(); setMessage(success); }
    catch (error) { setMessage(error instanceof Error ? error.message : "Request failed."); }
    finally { setBusy(false); }
  }

  async function loadPost(id: string) {
    const response = await checked(await fetch(`/api/posts/${id}`, { cache: "no-store" }));
    const post: Post = await response.json();
    const editable = { slug: post.slug, title: post.title, content: post.content, summary: post.summary,
      type: post.type, featured: post.featured, publishedAt: post.publishedAt };
    setSelected(post);
    setEtag(response.headers.get("etag"));
    setText(format(editable));
    setSavedText(format(editable));
    setEditing(true);
  }

  function discardChanges() {
    return !dirty || window.confirm("Discard your unsaved changes?");
  }

  function newPost() {
    if (!discardChanges()) return;
    setSelected(null);
    setEtag(null);
    setText(format(emptyPost));
    setSavedText(format(emptyPost));
    setEditing(true);
    setMessage("");
  }

  async function save() {
    const data = JSON.parse(text);
    if (!data || typeof data !== "object" || Array.isArray(data)) throw new Error("Enter a JSON object.");
    if (selected && !etag) throw new Error("Reload this post before saving: its version is missing.");
    const response = await checked(await fetch(selected ? `/api/posts/${selected.id}` : "/api/posts", {
      method: selected ? "PUT" : "POST",
      headers: { "Content-Type": "application/json", ...(selected && etag ? { "If-Match": etag } : {}) },
      body: JSON.stringify(data),
    }));
    const id = selected?.id ?? (await response.json()).id;
    // A successful write invalidates the old version even if reloading then fails.
    setEtag(null);
    setSavedText(text);
    setSelected({ id, title: typeof data.title === "string" ? data.title : "Post" });
    try {
      await loadPost(id);
      await loadPage(posts?.page ?? 1);
    } catch {
      throw new Error("The post was saved, but refreshing failed. Your JSON is preserved. Use Reload or refresh the post list before saving again.");
    }
  }

  async function remove() {
    if (!selected || !window.confirm(`Permanently delete "${selected.title}"?`)) return;
    await run(async () => {
      await checked(await fetch(`/api/posts/${selected.id}`, { method: "DELETE" }));
      setSelected(null); setEditing(false); setText(""); setSavedText(""); setEtag(null);
      await loadPage(1);
    }, "Deleted.");
  }

  const button = "cursor-pointer rounded border border-zinc-300 px-3 py-1.5 text-sm disabled:cursor-not-allowed disabled:opacity-40 dark:border-zinc-700";
  return (
    <section className="space-y-5">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Posts</h1>
        <button className={button} disabled={busy} onClick={newPost}>New post</button>
      </div>
      {message && <p role="status" className="whitespace-pre-wrap rounded border border-zinc-300 p-3 text-sm dark:border-zinc-700">{message}</p>}
      <div className="grid gap-8 md:grid-cols-[240px_1fr]">
        <aside className="space-y-4" aria-label="Post list">
          {busy && <p className="text-sm text-zinc-500">Working…</p>}
          {posts?.items.length === 0 && <p>No posts.</p>}
          <ul className="space-y-2">
            {posts?.items.map(post => <li key={post.id}>
              <button className={`w-full cursor-pointer text-left underline-offset-4 hover:underline disabled:opacity-50 ${selected?.id === post.id ? "font-semibold underline" : ""}`}
                disabled={busy} onClick={() => { if (discardChanges()) void run(() => loadPost(post.id)); }}>
                {post.title}
              </button>
            </li>)}
          </ul>
          <div className="flex flex-wrap items-center gap-2">
            <button className={button} disabled={busy || !posts || posts.page <= 1}
              onClick={() => void run(() => loadPage((posts?.page ?? 1) - 1))}>Previous</button>
            <button className={button} disabled={busy || !posts || posts.page * posts.pageSize >= posts.totalCount}
              onClick={() => void run(() => loadPage((posts?.page ?? 1) + 1))}>Next</button>
            <button className={button} disabled={busy} onClick={() => void run(() => loadPage(posts?.page ?? 1))}>Refresh</button>
          </div>
          {posts && <p className="text-xs text-zinc-500">Page {posts.page} · {posts.totalCount} posts</p>}
        </aside>
        {editing ? <div className="space-y-3">
          <label htmlFor="post-json" className="block text-sm font-medium">{selected ? "Edit post" : "New post"}{dirty ? " · unsaved" : ""}</label>
          {selected && <p className="break-all text-xs text-zinc-500">{selected.id}{selected.updatedAt ? ` · Updated ${selected.updatedAt}` : ""}</p>}
          <textarea id="post-json" value={text} onChange={event => setText(event.target.value)} disabled={busy}
            spellCheck={false} className="min-h-[28rem] w-full rounded border border-zinc-300 bg-transparent p-4 font-mono text-sm dark:border-zinc-700" />
          <div className="flex gap-2">
            <button className={button} disabled={busy || (!!selected && !etag)} onClick={() => void run(save, "Saved.")}>Save</button>
            {selected && <>
              <button className={button} disabled={busy} onClick={() => { if (discardChanges()) void run(() => loadPost(selected.id)); }}>Reload</button>
              <button className={button} disabled={busy} onClick={() => void remove()}>Delete</button>
            </>}
          </div>
          <p className="text-xs text-zinc-500">publishedAt: null = draft; future date = scheduled. All seven fields are required when editing.</p>
        </div> : <p className="text-sm text-zinc-500">Select a post or create one.</p>}
      </div>
    </section>
  );
}
