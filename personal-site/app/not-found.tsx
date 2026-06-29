import Link from "next/link";
import { SiteShell } from "@/app/_components/site-shell";

export const metadata = {
  title: "Page not found",
};

export default function NotFound() {
  return (
    <SiteShell>
      <main>
        <section className="section contact-section">
          <div className="section-inner">
            <p className="eyebrow">404</p>
            <h1 className="page-title">Page not found.</h1>
            <p className="lede">
              The link you followed may be broken, or the page may have been
              moved or removed.
            </p>
            <div className="button-row" style={{ marginTop: "2rem" }}>
              <Link href="/" className="button button-dark">
                Go home
              </Link>
              <Link href="/contact" className="button">
                Contact
              </Link>
            </div>
          </div>
        </section>
      </main>
    </SiteShell>
  );
}
