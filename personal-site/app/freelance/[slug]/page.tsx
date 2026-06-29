import Link from "next/link";
import { notFound } from "next/navigation";
import { ErrorState, SiteShell } from "@/app/_components/site-shell";
import { publicApi } from "@/app/_lib/api";

export default async function FreelanceDetailPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
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
                <Link
                  className="detail-back-link"
                  href="/freelance"
                  aria-label="Back to freelance"
                >
                  <svg
                    aria-hidden="true"
                    viewBox="0 0 24 24"
                    focusable="false"
                  >
                    <path d="M19 12H6m6-6-6 6 6 6" />
                  </svg>
                </Link>
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
