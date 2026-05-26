"use client";

import { useState } from "react";
import DesktopNavbar from "./DesktopNavbar";
import MobileNavbar from "./MobileNavbar";
import Card from "../ui/Card";

export default function Navbar() {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <nav className="py-4">
      <Card padding="sm" className="flex items-center justify-between gap-4">
        <div className="text-lg font-semibold tracking-normal text-slate-950">
          Eduard Teodor
        </div>

        <DesktopNavbar />

        <button
          type="button"
          aria-label="Toggle navigation"
          aria-expanded={isOpen}
          onClick={() => setIsOpen((open) => !open)}
          className="inline-flex size-10 items-center justify-center rounded-md text-slate-700 transition-colors hover:bg-slate-100 hover:text-slate-950 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-accent md:hidden"
        >
          <span className="flex flex-col gap-1.5">
            <span className="block h-0.5 w-5 rounded-full bg-current" />
            <span className="block h-0.5 w-5 rounded-full bg-current" />
            <span className="block h-0.5 w-5 rounded-full bg-current" />
          </span>
        </button>
      </Card>

      <MobileNavbar isOpen={isOpen} />
    </nav>
  );
}
