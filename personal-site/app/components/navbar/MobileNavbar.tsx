import NavLink from "./NavLink";
import {navigation} from "@/app/data/navigation";

interface MobileNavProps {
    isOpen: boolean;
}

export default function MobileNavbar({isOpen}: MobileNavProps) {
    if (!isOpen) return null;

    return (
        <ul className="md:hidden flex flex-col gap-2 py-4">
            {navigation.map((item) => (
                <NavLink key={item.href} href={item.href} mobile={true}>
                    {item.label}
                </NavLink>
            ))}
        </ul>
    )
    
}