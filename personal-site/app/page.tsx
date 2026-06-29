import Image from "next/image";
import Link from "next/link";
import {
  FreelanceCard,
  PostCard,
  ProjectCard,
} from "@/app/_components/content-cards";
import { EmptyState, ErrorState, SiteShell } from "@/app/_components/site-shell";
import { publicApi } from "@/app/_lib/api";

export default async function Home() {
  const [projects, freelance, posts] = await Promise.all([
    publicApi.projects(true),
    publicApi.freelance(true),
    publicApi.posts(true),
  ]);

  return (
    <SiteShell>
      <main>
        <section className="section hero-section">
          <div className="section-inner hero-wrap">
            <div className="hero-copy-block">
              <p className="eyebrow">London, United Kingdom</p>
              <h1 className="hero-title">Eduard Teodor</h1>
              <p className="hero-copy">
                Recent First Class BSc Computer Science graduate from {" "}
                <a className="inline-link" href="https://www.city.ac.uk/">
                  City, University of London
                </a>
                , currently looking for work and open to hiring conversations.
              </p>
              <div className="button-row">
                <Link href="/projects" className="button">
                  See projects
                </Link>
                <Link href="/freelance" className="button">
                  Freelance work
                </Link>
                <Link href="/contact" className="button button-dark">
                  Contact
                </Link>
              </div>
            </div>
            <div className="hero-portrait" aria-hidden="true">
              <Image
                src="/bitmoji-waving.webp"
                alt=""
                width={320}
                height={320}
                priority
              />
            </div>
          </div>
        </section>

        <section className="section muted">
          <div className="section-inner">
            <div className="stacked-section">
              <div>
                <p className="eyebrow">About</p>
                <h2>Grounded technical work, kept understandable.</h2>
              </div>
              <div className="prose">
                <p>
                  I&apos;m a Computer Science Graduate who enjoys building and supporting systems. 
                  Whether programming, designing, or handling client-facing work.
                </p>
                <p>
                  I&apos;m interested in practical digital systems that help people, small businesses, and teams manage, publish, automate, or make things easier to run.
                </p>
              </div>
            </div>
          </div>
        </section>

        <section className="section section-compact">
          <div className="section-inner">
            <div className="section-heading inline-heading">
              <div>
                <p className="eyebrow">Freelance</p>
                <h2>Small builds, workflows, and improvements.</h2>
              </div>
              <Link className="text-link" href="/freelance">
                See services
              </Link>
            </div>
            {freelance.ok ? (
              freelance.data.length ? (
                <div className="card-grid">
                  {freelance.data.slice(0, 3).map((item) => (
                    <FreelanceCard key={item.slug} item={item} />
                  ))}
                </div>
              ) : (
                <EmptyState title="No freelance examples yet">
                  Featured client work will appear here when it is published.
                </EmptyState>
              )
            ) : (
              <ErrorState message={freelance.message} />
            )}
          </div>
        </section>

        <section className="section section-compact muted">
          <div className="section-inner">
            <div className="section-heading inline-heading">
              <div>
                <p className="eyebrow">Personal Work</p>
                <h2>Projects</h2>
              </div>
              <Link className="text-link" href="/projects">
                View all projects
              </Link>
            </div>
            {projects.ok ? (
              projects.data.length ? (
                <div className="card-grid">
                  {projects.data.slice(0, 3).map((project) => (
                    <ProjectCard key={project.slug} project={project} />
                  ))}
                </div>
              ) : (
                <EmptyState title="No featured projects yet">
                  Published featured projects will appear here once they are
                  added in the admin area.
                </EmptyState>
              )
            ) : (
              <ErrorState message={projects.message} />
            )}
          </div>
        </section>

        <section className="section">
          <div className="section-inner">
            <div className="section-heading inline-heading">
              <div>
                <p className="eyebrow">Writing</p>
                <h2>Notes on building and learning.</h2>
              </div>
              <Link className="text-link" href="/blog">
                Read notes
              </Link>
            </div>
            {posts.ok ? (
              posts.data.length ? (
                <div className="card-grid">
                  {posts.data.slice(0, 3).map((post) => (
                    <PostCard key={post.slug} post={post} />
                  ))}
                </div>
              ) : (
                <EmptyState title="No notes published yet">
                  Starred notes will appear here once they are available.
                </EmptyState>
              )
            ) : (
              <ErrorState message={posts.message} />
            )}
          </div>
        </section>

        <section className="section cta-section muted">
          <div className="section-inner">
            <div className="stacked-section">
              <div>
                <p className="eyebrow">Contact</p>
                <h2>Hiring, collaborating, or need practical help?</h2>
                <p>
                  I am looking for work and open to roles, hiring conversations,
                  collaborations, and freelance projects. If you think I could
                  match what you need, send a short note.
                </p>
              </div>
              <Link href="/contact" className="button button-dark cta-button">
                Get in touch
              </Link>
            </div>
          </div>
        </section>
      </main>
    </SiteShell>
  );
}
