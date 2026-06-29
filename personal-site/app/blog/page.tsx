import { PostCard } from "@/app/_components/content-cards";
import {
  EmptyState,
  ErrorState,
  PageHeader,
  SiteShell,
} from "@/app/_components/site-shell";
import { publicApi } from "@/app/_lib/api";

export default async function BlogPage() {
  const posts = await publicApi.posts();

  return (
    <SiteShell>
      <main>
        <PageHeader eyebrow="Notes" title="Writing on useful technical work.">
          <p>
            Short notes on building, learning, and solving practical problems
            with software and web systems.
          </p>
        </PageHeader>
        <section className="section pt-0">
          <div className="section-inner">
            {posts.ok ? (
              posts.data.length ? (
                <div className="card-grid">
                  {posts.data.map((post) => (
                    <PostCard key={post.slug} post={post} />
                  ))}
                </div>
              ) : (
                <EmptyState title="No notes published yet">
                  Published writing will appear here once it is available.
                </EmptyState>
              )
            ) : (
              <ErrorState message={posts.message} />
            )}
          </div>
        </section>
      </main>
    </SiteShell>
  );
}
