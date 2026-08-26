import { Skeleton } from "@/shared/components/ui/skeleton";
import { Card, CardContent, CardHeader } from "@/shared/components/ui/card";

const ITEM_COUNT = 5;

export function DepartmentTreeSkeleton() {
  return (
    <div className="flex flex-col gap-1">
      {Array.from({ length: ITEM_COUNT }).map((_, index) => (
        <Card
          key={index}
          className="border-l-4 border-l-primary/10 mb-0.5"
        >
          <CardHeader className="flex flex-row items-center justify-between pb-2 mt-[-10]">
            <div className="flex items-center gap-2 min-w-0">
              <Skeleton className="size-4 shrink-0 rounded" />
              <Skeleton className="h-4 w-32" />
              <Skeleton className="h-5 w-16 rounded-full" />
            </div>
          </CardHeader>
          <CardContent className="pb-3">
            <div className="flex flex-wrap items-center gap-x-4 gap-y-1">
              <Skeleton className="h-3 w-16" />
              <Skeleton className="h-3 w-40" />
              <Skeleton className="h-3 w-28" />
              <Skeleton className="h-3 w-28" />
              <Skeleton className="size-3 rounded-full" />
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
