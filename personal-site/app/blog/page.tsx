import { CardCollection } from "@/app/_components/card-collection";
import { PostCard } from "@/app/_components/content-cards";
import { PageHeader, SiteShell } from "@/app/_components/site-shell";
import { publicApi } from "@/app/_lib/api";
import { pageMetadata } from "@/app/_lib/metadata";

export const metadata = pageMetadata({
  title: "Notes",
  description:
    "Short notes from Eduard Teodor on building, learning, and solving practical technical problems with software and web systems.",
});

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
            <CardCollection
              state={posts}
              emptyTitle="No notes published yet"
              renderItem={(post) => <PostCard key={post.slug} post={post} />}
            >
              Published writing will appear here once it is available.
            </CardCollection>
          </div>
        </section>
      </main>
    </SiteShell>
  );
}
