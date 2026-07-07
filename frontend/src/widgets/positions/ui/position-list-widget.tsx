"use client";

import { DepartmentMenu } from "@/features/departments";
import { usePositionList } from "@/features/positions/model/use-position-list";
import { PositionList } from "@/features/positions/ui/position-list";
import { NotFoundCard } from "@/shared/components/cards/not-found-card";
import { ErrorCard } from "@/shared/components/errors/error-card";
import { StatusFilter } from "@/shared/components/filters/status-filter";
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

  if (isError) {
    return <ErrorCard errors={errors ?? []} refetch={refetch} />;
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
          <DepartmentMenu onDepartmentIdsChange={setDepartmentIds}>Related departments</DepartmentMenu>
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
        <NotFoundCard title="Not found positions" description="Try adjusting your search or filters"/>
      )}
    </div>
  );
}
