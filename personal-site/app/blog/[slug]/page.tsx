import Link from "next/link";
import { notFound } from "next/navigation";
import { ErrorState, SiteShell } from "@/app/_components/site-shell";
import { publicApi } from "@/app/_lib/api";

export default async function BlogPostPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const post = await publicApi.post(slug);

  if (!post.ok && post.status === 404) {
    notFound();
  }

  const publishedDate =
    post.ok && post.data.publishedAt
      ? new Intl.DateTimeFormat("en-GB", {
          day: "numeric",
          month: "long",
          year: "numeric",
        }).format(new Date(post.data.publishedAt))
      : null;

  return (
    <SiteShell>
      <main className="section">
        <div className="section-inner detail-layout detail-layout-single">
          {post.ok ? (
            <article className="detail-article">
              <Link
                className="detail-back-link"
                href="/blog"
                aria-label="Back to notes"
              >
                <svg aria-hidden="true" viewBox="0 0 24 24" focusable="false">
                  <path d="M19 12H6m6-6-6 6 6 6" />
                </svg>
              </Link>
              <p className="eyebrow">Note</p>
              <h1 className="page-title">{post.data.title}</h1>
              <div className="detail-meta-panel">
                <div>
                  <h2>Writing</h2>
                  <ul className="pill-list">
                    <li>{publishedDate || "Published note"}</li>
                    {post.data.starred ? <li>Selected</li> : null}
                  </ul>
                </div>
              </div>
              {post.data.excerpt ? (
                <p className="lede">{post.data.excerpt}</p>
              ) : null}
              <div className="prose body-copy">
                {(post.data.content || "This note is being prepared.")
                  .split(/\n{2,}/)
                  .map((paragraph) => (
                    <p key={paragraph}>{paragraph}</p>
                  ))}
              </div>
            </article>
          ) : (
            <ErrorState message={post.message} />
          )}
        </div>
      </main>
    </SiteShell>
  );
}
