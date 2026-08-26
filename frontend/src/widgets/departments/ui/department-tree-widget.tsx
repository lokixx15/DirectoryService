"use client";

import { useState } from "react";
import { DepartmentSelect } from "@/features/departments/ui/department-select/department-select";
import { LocationMenu } from "@/features/locations/ui/location-menu";
import { useRootDepartments } from "@/features/departments/model/use-root-departments";
import { usePagination } from "@/shared/hooks/use-pagination";
import { DepartmentTree } from "@/features/departments/ui/department-tree/department-tree";
import { DepartmentTreeSkeleton } from "@/features/departments/ui/department-tree/department-tree-skeleton";
import { LoadMoreButton } from "@/shared/components/pagination/load-more-button";
import { ErrorCard } from "@/shared/components/errors/error-card";
import { PositionList } from "@/features/positions/ui/position-list";
import { usePositionList } from "@/features/positions/model/use-position-list";
import { SkeletonCard } from "@/shared/components/skeletons/skeleton-card";
import { NotFoundCard } from "@/shared/components/cards/not-found-card";
import {
  Alert,
  AlertDescription,
  AlertTitle,
} from "@/shared/components/ui/alert";
import { Info } from "lucide-react";

const DEFAULT_CHILDREN_LIMIT = 3;

export function DepartmentTreeWidget() {
  const [departmentIds, setDepartmentIds] = useState<string[]>([]);
  const [excludedDepartmentIds, setExcludedDepartmentIds] = useState<string[]>(
    [],
  );
  const [locationIds, setLocationIds] = useState<string[]>([]);

  const { page, pageSize, onPageSizeChange } = usePagination(5);

  const {
    departments,
    isError: isDepartmentError,
    errors: departmentErrors,
    isFetching: isDepartmentFetching,
    isPending: isDepartmentPending,
    refetch: refetchDepartments,
  } = useRootDepartments({
    page,
    size: pageSize,
    prefetch: DEFAULT_CHILDREN_LIMIT,
    departmentIds,
    excludedDepartmentIds,
  });

  const [selectedId, setSelectedId] = useState("");

  const {
    positions,
    errors: positionErrors,
    isFetching: isPositionFetching,
    isError: isPositionError,
    isFetchingNextPage,
    hasNextPage,
    fetchNextPage,
    refetch: positionRefetch,
  } = usePositionList({
    pageSize,
    departmentIds: [selectedId],
    enabled: !!selectedId,
  });

  if (isDepartmentPending) {
    return <DepartmentTreeSkeleton />;
  }

  if (isDepartmentError || departmentErrors) {
    return (
      <ErrorCard errors={departmentErrors ?? []} refetch={refetchDepartments} />
    );
  }

  const hasMore = departments
    ? departments.length >= pageSize * DEFAULT_CHILDREN_LIMIT
    : false;

  return (
    <div className="grid grid-cols-2 gap-10">
      <div className="flex flex-col gap-2">
        <DepartmentSelect
          addedDepartmentIds={departmentIds}
          onAddedDepartmentIdsChange={setDepartmentIds}
          excludedDepartmentIds={excludedDepartmentIds}
          onExcludedDepartmentIdsChange={setExcludedDepartmentIds}
          locationIds={locationIds}
          filterActions={
            <LocationMenu onLocationIdsChange={setLocationIds}>
              Related locations
            </LocationMenu>
          }
        />

        {departments && (
          <DepartmentTree
            departments={departments}
            selectedId={selectedId}
            onSelectedId={setSelectedId}
          />
        )}

        <LoadMoreButton
          pageSize={pageSize}
          onPageSizeChange={onPageSizeChange}
          loading={isDepartmentFetching}
          hasMore={hasMore}
        />
      </div>

      <div className="flex flex-col gap-3">
        {!selectedId ? (
          <Alert className="bg-muted/40 w-fit pr-5 mx-auto">
            <Info className="h-4 w-4 text-muted-foreground" />
            <AlertTitle>Positions</AlertTitle>
            <AlertDescription className="text-muted-foreground">
              Select any department to see positions.
            </AlertDescription>
          </Alert>
        ) : isPositionFetching ? (
          <SkeletonCard quantity={5} layoutClassName="grid grid-cols-1 gap-2"/>
        ) : isPositionError || (positionErrors && positionErrors.length > 0) ? (
          <ErrorCard errors={positionErrors ?? []} refetch={positionRefetch} />
        ) : positions?.length ? (
          <PositionList
            positions={positions}
            isFetchingNextPage={isFetchingNextPage}
            hasNextPage={hasNextPage}
            fetchNextPage={fetchNextPage}
            layoutClassName="flex flex-col gap-3"
          />
        ) : (
          <NotFoundCard title="Not found positions" />
        )}
      </div>
    </div>
  );
}
