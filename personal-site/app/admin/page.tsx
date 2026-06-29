"use client";

import Image from "next/image";
import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import {
  authHeaders,
  AuthResponse,
  BlogPost,
  FreelanceProject,
  getApiUrl,
  Project,
} from "@/app/_lib/api";

type AdminKind = "blog" | "projects" | "freelance";
type AdminItem = BlogPost | Project | FreelanceProject;

const resources: Record<
  AdminKind,
  { label: string; path: string; blank: AdminItem }
> = {
  blog: {
    label: "Blog posts",
    path: "/api/admin/blog-posts",
    blank: {
      slug: "",
      title: "",
      excerpt: "",
      content: "",
      starred: false,
      published: false,
    },
  },
  projects: {
    label: "Projects",
    path: "/api/admin/projects",
    blank: {
      slug: "",
      title: "",
      summary: "",
      description: "",
      techStack: "",
      status: "IN_PROGRESS",
      featured: false,
      published: false,
      displayOrder: 0,
    },
  },
  freelance: {
    label: "Freelance",
    path: "/api/admin/freelance-projects",
    blank: {
      slug: "",
      clientName: "",
      projectTitle: "",
      summary: "",
      description: "",
      services: [],
      featured: false,
      published: false,
      displayOrder: 0,
    },
  },
};

export default function AdminPage() {
  const [token, setToken] = useState("");
  const [preAuthToken, setPreAuthToken] = useState("");
  const [secret, setSecret] = useState<string | null>(null);
  const [qrCode, setQrCode] = useState<string | null>(null);
  const [step, setStep] = useState<"login" | "totp" | "authenticated">(
    "login",
  );
  const [message, setMessage] = useState("");

  useEffect(() => {
    const stored = window.localStorage.getItem("adminJwt");
    if (stored) {
      window.setTimeout(() => {
        setToken(stored);
        setStep("authenticated");
      }, 0);
    }
  }, []);

  function completeAuth(jwt: string) {
    setToken(jwt);
    window.localStorage.setItem("adminJwt", jwt);
    setPreAuthToken("");
    setSecret(null);
    setQrCode(null);
    setStep("authenticated");
    setMessage("");
  }

  async function submitLogin(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setMessage("Checking credentials...");
    const formData = new FormData(event.currentTarget);
    const response = await postAuth("/api/auth/login", {
      username: formData.get("username"),
      password: formData.get("password"),
    });

    if (!response) return;

    if (response.status === "AUTHENTICATED" && response.token) {
      completeAuth(response.token);
      return;
    }

    if (
      (response.status === "TOTP_REQUIRED" ||
        response.status === "TOTP_SETUP_REQUIRED") &&
      response.preAuthToken
    ) {
      setPreAuthToken(response.preAuthToken);
      setSecret(response.secret);
      setQrCode(response.qrCode);
      setStep("totp");
      setMessage(
        response.status === "TOTP_SETUP_REQUIRED"
          ? "Scan the QR code or enter the secret, then verify your code."
          : "Enter your authenticator code to finish signing in.",
      );
    }
  }

  async function submitTotp(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setMessage("Verifying code...");
    const formData = new FormData(event.currentTarget);
    const response = await postAuth("/api/auth/totp/verify", {
      preAuthToken,
      code: formData.get("code"),
    });

    if (response?.status === "AUTHENTICATED" && response.token) {
      completeAuth(response.token);
    }
  }

  async function postAuth(path: string, body: Record<string, FormDataEntryValue | string | null>) {
    try {
      const response = await fetch(getApiUrl(path), {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body),
      });

      if (!response.ok) {
        setMessage("Sign-in failed. Check your details and try again.");
        return null;
      }

      return (await response.json()) as AuthResponse;
    } catch {
      setMessage("The authentication service is not available right now.");
      return null;
    }
  }

  function signOut() {
    window.localStorage.removeItem("adminJwt");
    setToken("");
    setPreAuthToken("");
    setStep("login");
  }

  return (
    <main className="admin-page">
      <section className="admin-shell">
        <div className="admin-heading">
          <div>
            <p className="eyebrow">Admin</p>
            <h1>Content management</h1>
          </div>
          {step === "authenticated" ? (
            <button className="button" type="button" onClick={signOut}>
              Sign out
            </button>
          ) : null}
        </div>

        {step === "login" ? (
          <form className="admin-form" onSubmit={submitLogin}>
            <label>
              Username
              <input name="username" autoComplete="username" required />
            </label>
            <label>
              Password
              <input
                name="password"
                type="password"
                autoComplete="current-password"
                required
              />
            </label>
            <button className="button button-dark" type="submit">
              Continue
            </button>
          </form>
        ) : null}

        {step === "totp" ? (
          <form className="admin-form" onSubmit={submitTotp}>
            {qrCode ? (
              <Image
                className="qr-code"
                src={`data:image/png;base64,${qrCode}`}
                alt="Authenticator setup QR code"
                width={192}
                height={192}
                unoptimized
              />
            ) : null}
            {secret ? (
              <p className="secret-box">
                Manual secret: <strong>{secret}</strong>
              </p>
            ) : null}
            <label>
              Authenticator code
              <input
                name="code"
                inputMode="numeric"
                pattern="[0-9]{6}"
                autoComplete="one-time-code"
                required
              />
            </label>
            <button className="button button-dark" type="submit">
              Verify and sign in
            </button>
          </form>
        ) : null}

        {message ? <p className="admin-message">{message}</p> : null}

        {step === "authenticated" ? <AdminWorkspace token={token} /> : null}
      </section>
    </main>
  );
}

function AdminWorkspace({ token }: { token: string }) {
  const [kind, setKind] = useState<AdminKind>("blog");
  const config = resources[kind];

  return (
    <section className="admin-workspace">
      <div className="tab-row" role="tablist" aria-label="Content type">
        {(Object.keys(resources) as AdminKind[]).map((resource) => (
          <button
            key={resource}
            className={resource === kind ? "active" : ""}
            type="button"
            onClick={() => setKind(resource)}
          >
            {resources[resource].label}
          </button>
        ))}
      </div>
      <ResourceEditor key={kind} token={token} config={config} />
    </section>
  );
}

function ResourceEditor({
  token,
  config,
}: {
  token: string;
  config: { label: string; path: string; blank: AdminItem };
}) {
  const [items, setItems] = useState<AdminItem[]>([]);
  const [selectedId, setSelectedId] = useState<number | null>(null);
  const [draft, setDraft] = useState(() => stringify(config.blank));
  const [status, setStatus] = useState("Loading content...");

  const selected = useMemo(
    () => items.find((item) => item.id === selectedId),
    [items, selectedId],
  );

  const loadItems = useCallback(async () => {
    setStatus("Loading content...");
    try {
      const response = await fetch(getApiUrl(config.path), {
        headers: authHeaders(token),
      });
      if (!response.ok) {
        setStatus("Could not load admin content. Your session may have expired.");
        return;
      }
      const data = (await response.json()) as AdminItem[];
      setItems(data);
      setStatus(data.length ? "" : "No items yet.");
    } catch {
      setStatus("The admin API is not available right now.");
    }
  }, [config.path, token]);

  useEffect(() => {
    const timer = window.setTimeout(() => {
      loadItems();
    }, 0);

    return () => window.clearTimeout(timer);
  }, [loadItems]);

  function startCreate() {
    setSelectedId(null);
    setDraft(stringify(config.blank));
  }

  function startEdit(item: AdminItem) {
    setSelectedId(item.id ?? null);
    setDraft(stringify(item));
  }

  async function saveDraft() {
    let payload: AdminItem;
    try {
      payload = JSON.parse(draft) as AdminItem;
    } catch {
      setStatus("The JSON is not valid.");
      return;
    }

    const isUpdate = Boolean(selected?.id);
    const path = isUpdate ? `${config.path}/${selected?.id}` : config.path;
    const response = await fetch(getApiUrl(path), {
      method: isUpdate ? "PUT" : "POST",
      headers: {
        "Content-Type": "application/json",
        ...authHeaders(token),
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      setStatus(`Save failed with status ${response.status}.`);
      return;
    }

    setStatus("Saved.");
    await loadItems();
  }

  async function deleteSelected() {
    if (!selected?.id) {
      setStatus("Choose an existing item before deleting.");
      return;
    }

    const response = await fetch(getApiUrl(`${config.path}/${selected.id}`), {
      method: "DELETE",
      headers: authHeaders(token),
    });

    if (!response.ok) {
      setStatus(`Delete failed with status ${response.status}.`);
      return;
    }

    setSelectedId(null);
    setDraft(stringify(config.blank));
    setStatus("Deleted.");
    await loadItems();
  }

  return (
    <div className="resource-grid">
      <aside className="resource-list">
        <div className="resource-list-heading">
          <h2>{config.label}</h2>
          <button className="button" type="button" onClick={startCreate}>
            New
          </button>
        </div>
        {items.map((item) => (
          <button
            className={item.id === selectedId ? "resource-item active" : "resource-item"}
            key={`${item.slug}-${item.id}`}
            type="button"
            onClick={() => startEdit(item)}
          >
            <strong>{"title" in item ? item.title : item.projectTitle}</strong>
            <span>{item.slug}</span>
          </button>
        ))}
      </aside>
      <section className="json-editor">
        <label>
          JSON editor
          <textarea
            value={draft}
            onChange={(event) => setDraft(event.target.value)}
            spellCheck={false}
          />
        </label>
        <div className="button-row">
          <button className="button button-dark" type="button" onClick={saveDraft}>
            Save
          </button>
          <button className="button" type="button" onClick={deleteSelected}>
            Delete
          </button>
          <button className="button" type="button" onClick={loadItems}>
            Refresh
          </button>
        </div>
        {status ? <p className="admin-message">{status}</p> : null}
      </section>
    </div>
  );
}

function stringify(value: unknown) {
  return JSON.stringify(value, null, 2);
}
