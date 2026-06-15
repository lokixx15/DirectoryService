import { Position } from "@/entities/positions/types";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card";
import { Spinner } from "@/shared/components/ui/spinner";
import { FormatDate } from "@/shared/lib/format-date";
import { cn } from "@/shared/lib/utils";
import { RefCallback, useCallback } from "react";

interface PositionListProps {
  positions?: Position[];
  isFetchingNextPage: boolean;
  hasNextPage: boolean;
  fetchNextPage: () => void;
}

export function PositionList({
  positions,
  isFetchingNextPage,
  hasNextPage,
  fetchNextPage,
}: PositionListProps) {
  const cursorRef: RefCallback<HTMLDivElement> = useCallback(
    (el) => {
      const observer = new IntersectionObserver(
        (entries) => {
          if (entries[0].isIntersecting && hasNextPage && !isFetchingNextPage) {
            fetchNextPage();
          }
        },
        {
          threshold: 0.5,
        },
      );

      if (el) {
        observer.observe(el);

        return () => observer.disconnect();
      }
    },
    [fetchNextPage, hasNextPage, isFetchingNextPage],
  );

  return (
    <div className="flex flex-col gap-5">
      <div className="grid sm:grid-cols-1 md:grid-cols-2 gap-3 lg:grid-cols-3">
        {positions?.map((p) => {
          return (
            <Card
              key={p.id}
              className={cn(
                "transition-all duration-200 hover:shadow-md hover:-translate-y-0.5",
                p.isActive
                  ? "border-l-4 border-l-primary"
                  : "border-l-4 border-l-muted-foreground/20",
              )}
            >
              <CardHeader className="pb-3">
                <div className="flex items-start justify-between gap-2">
                  <CardTitle>{p.name}</CardTitle>
                  <span
                    className={cn(
                      "shrink-0 rounded-full px-2.5 py-0.5 text-xs font-medium",
                      p.isActive
                        ? "bg-primary/10 text-primary"
                        : "bg-muted text-muted-foreground",
                    )}
                  >
                    {p.isActive ? "Active" : "Inactive"}
                  </span>
                </div>
                {p.description && (
                  <p className="text-sm text-muted-foreground line-clamp-2">
                    {p.description}
                  </p>
                )}
              </CardHeader>
              <CardContent>
                <div className="flex flex-col gap-x-4 gap-y-1.5 text-xs">
                  <span>Created: {FormatDate(p.createdAt)}</span>
                  <span>Updated: {FormatDate(p.updatedAt)}</span>
                  {p.deletedAt && (
                    <span>Deleted: {FormatDate(p.deletedAt)}</span>
                  )}
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      <div ref={cursorRef} className="flex justify-center py-4">
        {isFetchingNextPage && <Spinner />}
      </div>
    </div>
  );
}
