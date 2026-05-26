import { cn } from "@/app/lib/cn";

interface CardProps {
  children: React.ReactNode;
  className?: string;
  padding?: "none" | "sm" | "md";
}

const paddingClasses = {
  none: "",
  sm: "p-4",
  md: "p-6",
};

export default function Card({
  children,
  className,
  padding = "md",
}: CardProps) {
  return (
    <div
      className={cn(
        "rounded-lg border border-black/20 bg-surface/95 text-surface-foreground shadow-lg backdrop-blur",
        paddingClasses[padding],
        className,
      )}
    >
      {children}
    </div>
  );
}
