"use client";

import { DepartmentMenu } from "@/features/departments";
import { StatusFilter } from "@/features/locations";
import { usePositionList } from "@/features/positions/model/use-position-list";
import { PositionList } from "@/features/positions/ui/position-list";
import { ErrorCard } from "@/shared/components/errors/error-card";
import { SearchBar } from "@/shared/components/search/search-bar";
import { SkeletonCard } from "@/shared/components/skeletons/skeleton-card";
import { Card, CardContent } from "@/shared/components/ui/card";
import { SearchX } from "lucide-react";
import { useState } from "react";

const PAGE_SIZE = 10;

export function PositionListWidget() {
  const [search, setSearch] = useState("");
  const [isActive, setIsActive] = useState<boolean>();
  const [departmentIds, setDepartmentIds] = useState<string[]>([]);

  const {
    positions,
    errors,
    isError,
    isPending,
    isFetchingNextPage,
    hasNextPage,
    fetchNextPage,
    refetch,
  } = usePositionList({
    pageSize: PAGE_SIZE,
    departmentIds: departmentIds,
    search: search,
    isActive: isActive,
  });

  if (isError && errors) {
    return <ErrorCard errors={errors} refetch={refetch} />;
  }

  if (isPending && !positions) {
    return <SkeletonCard quantity={6} />;
  }

  return (
    <div>
      <div className="flex justify-between mb-3">
        <SearchBar onSearch={setSearch} />
        <div className="flex gap-2">
          <StatusFilter onIsActive={setIsActive} />
          <DepartmentMenu onDepartmentIdsChange={setDepartmentIds} />
        </div>
      </div>
      {positions?.length ? (
        <PositionList
          positions={positions}
          isFetchingNextPage={isFetchingNextPage}
          hasNextPage={hasNextPage}
          fetchNextPage={fetchNextPage}
        />
      ) : (
        <Card className="border-muted-foreground/20">
          <CardContent className="flex flex-col items-center justify-center gap-2 py-16">
            <SearchX className="size-12 text-muted-foreground/50" />
            <p className="text-lg font-medium text-muted-foreground">
              No positions found
            </p>
            <p className="text-sm text-muted-foreground/60">
              Try adjusting your search or filters
            </p>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
