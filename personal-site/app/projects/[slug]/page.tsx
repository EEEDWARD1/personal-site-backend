import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { BackLink, ErrorState, SiteShell } from "@/app/_components/site-shell";
import { publicApi } from "@/app/_lib/api";
import { splitCommaList } from "@/app/_lib/format";
import { pageMetadata } from "@/app/_lib/metadata";

type ProjectDetailProps = {
  params: Promise<{ slug: string }>;
};

export async function generateMetadata({
  params,
}: ProjectDetailProps): Promise<Metadata> {
  const { slug } = await params;
  const project = await publicApi.project(slug);

  if (!project.ok) {
    return pageMetadata({
      title: "Project not found",
      description: "The requested project could not be found.",
    });
  }

  return pageMetadata({
    title: project.data.title,
    description:
      project.data.summary ||
      "A practical software or web project by Eduard Teodor.",
  });
}

export default async function ProjectDetailPage({ params }: ProjectDetailProps) {
  const { slug } = await params;
  const project = await publicApi.project(slug);

  if (!project.ok && project.status === 404) {
    notFound();
  }

  const tools = project.ok ? splitCommaList(project.data.techStack) : [];

  return (
    <SiteShell>
      <main className="section">
        <div className="section-inner detail-layout detail-layout-single">
          {project.ok ? (
            <>
              <article className="detail-article">
                <BackLink href="/projects" label="Back to projects" />
                <p className="eyebrow">Case study</p>
                <h1 className="page-title">{project.data.title}</h1>
                <div className="detail-meta-panel">
                  {tools.length ? (
                    <div>
                      <h2>Tools used</h2>
                      <ul className="pill-list">
                        {tools.map((tool) => (
                          <li key={tool}>{tool}</li>
                        ))}
                      </ul>
                    </div>
                  ) : null}
                  <div className="detail-actions">
                    {project.data.liveUrl ? (
                      <a className="button button-dark" href={project.data.liveUrl}>
                        Visit live work
                      </a>
                    ) : null}
                    {project.data.githubUrl ? (
                      <a className="button" href={project.data.githubUrl}>
                        View source
                      </a>
                    ) : null}
                  </div>
                </div>
                <p className="lede">
                  {project.data.summary ||
                    "A practical system built around a clear workflow."}
                </p>
                <section>
                  <h2>Problem</h2>
                  <p>
                    {project.data.description ||
                      "This work focused on making a digital workflow clearer, more reliable, and easier to manage."}
                  </p>
                </section>
                <section>
                  <h2>Solution</h2>
                  <p>
                    {project.data.summary ||
                      "The solution combines a public-facing experience with maintainable structure behind the scenes."}
                  </p>
                </section>
                <section>
                  <h2>Outcome</h2>
                  <p>
                    A more usable system with clearer content paths, practical
                    administration, and a foundation that can be maintained
                    after handoff.
                  </p>
                </section>
              </article>
            </>
          ) : (
            <ErrorState message={project.message} />
          )}
        </div>
      </main>
    </SiteShell>
  );
}
