import { Skeleton } from "../ui/skeleton";

export function SkeletonTable() {
  return (
    <div className="flex w-full flex-col gap-2">
      {Array.from({ length: 10 }).map((_, index) => (
        <div className="flex gap-4" key={index}>
          <Skeleton className="h-8 flex-1 bg-primary/10" />
          <Skeleton className="h-8 w-24 bg-primary/10" />
          <Skeleton className="h-8 w-20 bg-primary/10" />
        </div>
      ))}
    </div>
  );
}
