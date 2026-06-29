import { CardCollection } from "@/app/_components/card-collection";
import { FreelanceCard } from "@/app/_components/content-cards";
import { SiteShell } from "@/app/_components/site-shell";
import { publicApi } from "@/app/_lib/api";
import { pageMetadata } from "@/app/_lib/metadata";

export const metadata = pageMetadata({
  title: "Freelance",
  description:
    "Focused freelance help from Eduard Teodor for websites, admin tools, dashboards, hosting setup, and workflow improvements.",
});

export default async function FreelancePage() {
  const items = await publicApi.freelance();

  return (
    <SiteShell>
      <main>
        <section className="section freelance-hero">
          <div className="section-inner freelance-hero-inner">
            <div>
              <p className="eyebrow">Freelance</p>
              <h1 className="page-title">Focused help for practical digital work.</h1>
              <p className="lede">
                For people and small teams who need a website, workflow, admin
                tool, deployment path, or data process to work properly.
              </p>
            </div>
            <div className="freelance-note">
              <p>
                I am open to freelance projects across websites, admin tools,
                dashboards, hosting setup, and practical workflow improvements.
                Enquire with what you need, and I will let you know honestly
                whether it matches what I can help with.
              </p>
              <a className="button button-dark" href="/contact">
                Enquire
              </a>
            </div>
          </div>
        </section>
        <section className="section muted">
          <div className="section-inner">
            <div className="section-heading">
              <p className="eyebrow">Examples</p>
              <h2>Published freelance work.</h2>
            </div>
            <CardCollection
              state={items}
              emptyTitle="No examples published yet"
              renderItem={(item) => (
                <FreelanceCard key={item.slug} item={item} />
              )}
            >
              Freelance examples will appear here when they are available.
            </CardCollection>
          </div>
        </section>
      </main>
    </SiteShell>
  );
}
