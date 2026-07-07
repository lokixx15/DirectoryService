import { SearchX } from "lucide-react";
import { Card, CardContent } from "../ui/card";

interface NotFoundCardProps {
  title: string;
  description?: string;
}

export function NotFoundCard({ title, description }: NotFoundCardProps) {
  return (
    <Card className="border-muted-foreground/20 w-full">
      <CardContent className="flex flex-col items-center justify-center gap-2 py-16">
        <SearchX className="size-12 text-muted-foreground/50" />
        <p className="text-lg font-medium text-muted-foreground">{title}</p>
        {description && (
          <p className="text-sm text-muted-foreground/60">{description}</p>
        )}
      </CardContent>
    </Card>
  );
}
