import Image from "next/image";
import Card from "@/app/components/ui/Card";

export default function Home() {
  return (
    <section className="py-6 sm:py-10">
      <div className="grid gap-4 lg:grid-cols-[3fr_2fr]">
        <Card className="flex min-h-80 items-center justify-center">
          Left content
        </Card>

        <Card padding="none" className="overflow-hidden">
          <Image
            src="/image.webp"
            width={500}
            height={500}
            sizes="(min-width: 1024px) 36vw, 100vw"
            className="h-full min-h-80 w-full object-cover"
            alt="Picture of the author"
            priority
          />
        </Card>
      </div>
    </section>
  );
}
