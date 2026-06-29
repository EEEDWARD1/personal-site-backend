import { SiteShell } from "@/app/_components/site-shell";

export default function ContactPage() {
  return (
    <SiteShell>
      <main className="contact-page">
        <section className="section contact-section">
          <div className="section-inner contact-layout">
            <div className="contact-intro">
              <p className="eyebrow">Contact</p>
              <h1>Interested in hiring me or working together?</h1>
              <p>
                I am currently looking for work and open to roles,
                collaborations, and freelance projects in London or remote. If
                you are interested in hiring me, or want to enquire about a
                project, send a short note and I will let you know whether I can
                match what you need.
              </p>
            </div>

            <div className="contact-panel" aria-label="Contact details">
              <a className="contact-row" href="mailto:hello@eduardteodor.co.uk">
                <span>Email</span>
                <strong>hello@eduardteodor.co.uk</strong>
              </a>
              <a
                className="contact-row"
                href="https://linkedin.com/in/eduardteodor"
              >
                <span>LinkedIn</span>
                <strong>eduardteodor</strong>
              </a>
              <div className="contact-row">
                <span>Based in</span>
                <strong>London, United Kingdom</strong>
              </div>
            </div>
          </div>
        </section>
      </main>
    </SiteShell>
  );
}
