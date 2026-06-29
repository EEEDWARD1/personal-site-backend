"use client";

import Link from "next/link";
import { useState } from "react";

const navItems = [
  { href: "/", label: "Home" },
  { href: "/projects", label: "Projects" },
  { href: "/freelance", label: "Freelance" },
  { href: "/blog", label: "Notes" },
];

export function MobileMenu() {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div className="mobile-menu">
      <button
        className="mobile-menu-button"
        type="button"
        aria-label={isOpen ? "Close menu" : "Open menu"}
        aria-expanded={isOpen}
        aria-controls="mobile-menu-panel"
        onClick={() => setIsOpen((current) => !current)}
      >
        <span />
        <span />
        <span />
      </button>
      {isOpen ? (
        <div id="mobile-menu-panel" className="mobile-menu-panel">
          {navItems.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              onClick={() => setIsOpen(false)}
            >
              {item.label}
            </Link>
          ))}
          <Link
            className="mobile-menu-contact"
            href="/contact"
            onClick={() => setIsOpen(false)}
          >
            Get in touch
          </Link>
        </div>
      ) : null}
    </div>
  );
}
