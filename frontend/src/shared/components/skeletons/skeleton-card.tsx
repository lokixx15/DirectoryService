import { Card, CardContent, CardHeader } from "@/shared/components/ui/card";
import { Skeleton } from "@/shared/components/ui/skeleton";
import { cn } from "@/shared/lib/utils";

interface SkeletonCardProps {
  quantity: number;
  layoutClassName?: string;
}

export function SkeletonCard({ quantity, layoutClassName }: SkeletonCardProps) {
  return (
    <div
      className={cn(
        layoutClassName ??
          "grid sm:grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4",
      )}
    >
      {Array.from({ length: quantity }).map((_, index) => (
        <Card key={index} className="w-full">
          <CardHeader>
            <Skeleton className="h-4 w-2/3" />
            <Skeleton className="h-4 w-1/2" />
          </CardHeader>
          <CardContent>
            <Skeleton className="h-10 w-1/2" />
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
