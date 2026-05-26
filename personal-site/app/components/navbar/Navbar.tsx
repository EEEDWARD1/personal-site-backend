"use client";

import { useState } from "react";
import DesktopNavbar from "./DesktopNavbar";
import MobileNavbar from "./MobileNavbar";
import Card from "../ui/Card";

export default function Navbar() {
    const [isOpen, setIsOpen] = useState(false);

    return (
        <nav className="bg-gray-800 px-6 py-4">
            <Card className="mx-auto flex max-w-7xl items-center justify-between">
                
                {/* Logo */}
                <div className="text-xl font-bold text-black">
                    Eduard
                </div>

                {/* Desktop Navigation */}
                <DesktopNavbar />

                {/* Mobile Button */}
                <button
                    onClick={() => setIsOpen(!isOpen)}
                    className="text-black md:hidden"
                >
                    ☰
                </button>
            </Card>

            {/* Mobile Navigation */}
            <MobileNavbar isOpen={isOpen} />
        </nav>
    );
}