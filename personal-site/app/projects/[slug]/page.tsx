import Link from "next/link";
import { notFound } from "next/navigation";
import { ErrorState, SiteShell } from "@/app/_components/site-shell";
import { publicApi } from "@/app/_lib/api";

export default async function ProjectDetailPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const project = await publicApi.project(slug);

  if (!project.ok && project.status === 404) {
    notFound();
  }

  const tools = project.ok
    ? project.data.techStack
        ?.split(",")
        .map((tool) => tool.trim())
        .filter(Boolean) ?? []
    : [];

  return (
    <SiteShell>
      <main className="section">
        <div className="section-inner detail-layout detail-layout-single">
          {project.ok ? (
            <>
              <article className="detail-article">
                <Link
                  className="detail-back-link"
                  href="/projects"
                  aria-label="Back to projects"
                >
                  <svg
                    aria-hidden="true"
                    viewBox="0 0 24 24"
                    focusable="false"
                  >
                    <path d="M19 12H6m6-6-6 6 6 6" />
                  </svg>
                </Link>
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
