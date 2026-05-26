import NavLink from "./NavLink";
import Card from "../ui/Card";
import { navigation } from "@/app/data/navigation";

interface MobileNavProps {
  isOpen: boolean;
}

export default function MobileNavbar({ isOpen }: MobileNavProps) {
  if (!isOpen) return null;

  return (
    <Card padding="sm" className="mt-2 md:hidden">
      <ul className="flex flex-col gap-1">
        {navigation.map((item) => (
          <NavLink key={item.href} href={item.href} mobile>
            {item.label}
          </NavLink>
        ))}
      </ul>
    </Card>
  );
}
