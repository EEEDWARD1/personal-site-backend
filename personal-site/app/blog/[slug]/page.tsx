import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { BackLink, ErrorState, SiteShell } from "@/app/_components/site-shell";
import { publicApi } from "@/app/_lib/api";
import { formatDisplayDate, splitParagraphs } from "@/app/_lib/format";
import { pageMetadata } from "@/app/_lib/metadata";

type BlogPostProps = {
  params: Promise<{ slug: string }>;
};

export async function generateMetadata({
  params,
}: BlogPostProps): Promise<Metadata> {
  const { slug } = await params;
  const post = await publicApi.post(slug);

  if (!post.ok) {
    return pageMetadata({
      title: "Note not found",
      description: "The requested note could not be found.",
    });
  }

  return pageMetadata({
    title: post.data.title,
    description:
      post.data.excerpt ||
      "A note from Eduard Teodor on building and learning with software.",
  });
}

export default async function BlogPostPage({ params }: BlogPostProps) {
  const { slug } = await params;
  const post = await publicApi.post(slug);

  if (!post.ok && post.status === 404) {
    notFound();
  }

  const publishedDate = post.ok
    ? formatDisplayDate(post.data.publishedAt)
    : null;

  return (
    <SiteShell>
      <main className="section">
        <div className="section-inner detail-layout detail-layout-single">
          {post.ok ? (
            <article className="detail-article">
              <BackLink href="/blog" label="Back to notes" />
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
                {splitParagraphs(
                  post.data.content,
                  "This note is being prepared.",
                ).map((paragraph) => (
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
