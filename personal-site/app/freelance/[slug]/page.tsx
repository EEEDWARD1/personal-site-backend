import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { BackLink, ErrorState, SiteShell } from "@/app/_components/site-shell";
import { publicApi } from "@/app/_lib/api";
import { pageMetadata } from "@/app/_lib/metadata";

type FreelanceDetailProps = {
  params: Promise<{ slug: string }>;
};

export async function generateMetadata({
  params,
}: FreelanceDetailProps): Promise<Metadata> {
  const { slug } = await params;
  const item = await publicApi.freelanceProject(slug);

  if (!item.ok) {
    return pageMetadata({
      title: "Freelance work not found",
      description: "The requested freelance project could not be found.",
    });
  }

  return pageMetadata({
    title: item.data.projectTitle,
    description:
      item.data.summary ||
      "A focused freelance digital project by Eduard Teodor.",
  });
}

export default async function FreelanceDetailPage({
  params,
}: FreelanceDetailProps) {
  const { slug } = await params;
  const item = await publicApi.freelanceProject(slug);

  if (!item.ok && item.status === 404) {
    notFound();
  }

  return (
    <SiteShell>
      <main className="section">
        <div className="section-inner detail-layout detail-layout-single">
          {item.ok ? (
            <>
              <article className="detail-article">
                <BackLink href="/freelance" label="Back to freelance" />
                <p className="eyebrow">{item.data.clientName || "Client work"}</p>
                <h1 className="page-title">{item.data.projectTitle}</h1>
                <div className="detail-meta-panel">
                  {item.data.services?.length ? (
                    <div>
                      <h2>Services</h2>
                      <ul className="pill-list">
                        {item.data.services.map((service) => (
                          <li key={service}>{service}</li>
                        ))}
                      </ul>
                    </div>
                  ) : null}
                  <div className="detail-actions">
                    {item.data.websiteUrl ? (
                      <a className="button button-dark" href={item.data.websiteUrl}>
                        Visit website
                      </a>
                    ) : null}
                  </div>
                </div>
                <p className="lede">
                  {item.data.summary || "A focused practical digital build."}
                </p>
                <section>
                  <h2>Problem</h2>
                  <p>
                    {item.data.description ||
                      "The project needed a clearer digital workflow and a dependable way to manage the result."}
                  </p>
                </section>
                <section>
                  <h2>Solution</h2>
                  <p>
                    The work focused on a simple, maintainable system that
                    matched the client&apos;s immediate needs and could be
                    handed off clearly.
                  </p>
                </section>
                {item.data.testimonial ? (
                  <blockquote>{item.data.testimonial}</blockquote>
                ) : null}
              </article>
            </>
          ) : (
            <ErrorState message={item.message} />
          )}
        </div>
      </main>
    </SiteShell>
  );
}
