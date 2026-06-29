import { ProjectCard } from "@/app/_components/content-cards";
import {
  EmptyState,
  ErrorState,
  PageHeader,
  SiteShell,
} from "@/app/_components/site-shell";
import { publicApi } from "@/app/_lib/api";

export default async function ProjectsPage() {
  const projects = await publicApi.projects();

  return (
    <SiteShell>
      <main className="projects-page">
        <PageHeader eyebrow="Projects" title="Work shaped around real use.">
          <p>
            A small collection of software and web systems, framed around what
            they help manage, publish, automate, or make easier to run.
          </p>
        </PageHeader>
        <section className="section pt-0">
          <div className="section-inner">
            {projects.ok ? (
              projects.data.length ? (
                <div className="card-grid">
                  {projects.data.map((project) => (
                    <ProjectCard key={project.slug} project={project} />
                  ))}
                </div>
              ) : (
                <EmptyState title="No projects published yet">
                  Published work will appear here once it is added.
                </EmptyState>
              )
            ) : (
              <ErrorState message={projects.message} />
            )}
          </div>
        </section>
      </main>
    </SiteShell>
  );
}
