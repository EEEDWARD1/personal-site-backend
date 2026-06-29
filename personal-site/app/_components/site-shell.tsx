import Link from "next/link";
import type { ReactNode } from "react";
import { MobileMenu } from "@/app/_components/mobile-menu";

const navItems = [
  { href: "/", label: "Home" },
  { href: "/projects", label: "Projects" },
  { href: "/freelance", label: "Freelance" },
  { href: "/blog", label: "Notes" },
  { href: "/contact", label: "Contact" },
];

export function SiteShell({ children }: { children: ReactNode }) {
  return (
    <div className="flex min-h-dvh flex-col bg-stone-50 text-zinc-950">
      <header className="sticky top-0 z-20 border-b border-zinc-200/80 bg-stone-50/90 backdrop-blur">
        <nav
          className="mx-auto flex max-w-6xl items-center justify-between gap-6 px-5 py-4"
          aria-label="Main navigation"
        >
          <Link href="/" className="font-semibold tracking-tight">
            Eduard Teodor
          </Link>
          <div className="hidden items-center gap-1 md:flex">
            {navItems.map((item) => (
              <Link key={item.href} className="nav-link" href={item.href}>
                {item.label}
              </Link>
            ))}
          </div>
          <Link className="button button-dark header-cta" href="/contact">
            Get in touch
          </Link>
          <MobileMenu />
        </nav>
      </header>
      <div className="flex flex-1 flex-col">{children}</div>
      <footer className="mt-auto border-t border-zinc-200 bg-white">
        <div className="mx-auto grid max-w-6xl gap-6 px-5 py-10 text-sm text-zinc-600 md:grid-cols-[1fr_auto]">
          <p>
            London-based recent Computer Science graduate building practical
            digital systems for people, small businesses, and teams.
          </p>
          <div className="flex flex-wrap gap-3">
            <Link href="/admin">Admin</Link>
            <a href="mailto:hello@eduardteodor.co.uk">
              hello@eduardteodor.co.uk
            </a>
          </div>
        </div>
      </footer>
    </div>
  );
}

export function BackLink({
  href,
  label,
}: {
  href: string;
  label: string;
}) {
  return (
    <Link className="detail-back-link" href={href} aria-label={label}>
      <svg aria-hidden="true" viewBox="0 0 24 24" focusable="false">
        <path d="M19 12H6m6-6-6 6 6 6" />
      </svg>
    </Link>
  );
}

export function PageHeader({
  eyebrow,
  title,
  children,
}: {
  eyebrow?: string;
  title: string;
  children: ReactNode;
}) {
  return (
    <section className="section pt-16 md:pt-24">
      <div className="section-inner max-w-3xl">
        {eyebrow ? <p className="eyebrow">{eyebrow}</p> : null}
        <h1 className="page-title">{title}</h1>
        <div className="lede">{children}</div>
      </div>
    </section>
  );
}

export function EmptyState({
  title,
  children,
}: {
  title: string;
  children: ReactNode;
}) {
  return (
    <div className="state-box">
      <h2>{title}</h2>
      <p>{children}</p>
    </div>
  );
}

export function ErrorState({ message }: { message: string }) {
  return (
    <div className="state-box border-rose-200 bg-rose-50 text-rose-950">
      <h2>Content unavailable</h2>
      <p>{message}</p>
    </div>
  );
}
