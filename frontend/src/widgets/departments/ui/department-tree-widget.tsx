"use client";

import { useRootDepartments } from "@/features/departments/model/use-root-departments";
import { DepartmentSelect } from "@/features/departments/ui/department-select/department-select";
import { DepartmentTree } from "@/features/departments/ui/department-tree/department-tree";
import { DepartmentTreeSkeleton } from "@/features/departments/ui/department-tree/department-tree-skeleton";
import { LocationMenu } from "@/features/locations/ui/location-menu";
import { usePositionList } from "@/features/positions/model/use-position-list";
import { PositionList } from "@/features/positions/ui/position-list";
import { NotFoundCard } from "@/shared/components/cards/not-found-card";
import { ErrorCard } from "@/shared/components/errors/error-card";
import { LoadMoreButton } from "@/shared/components/pagination/load-more-button";
import { SkeletonCard } from "@/shared/components/skeletons/skeleton-card";
import {
  Alert,
  AlertDescription,
  AlertTitle,
} from "@/shared/components/ui/alert";
import { Toggle } from "@/shared/components/ui/toggle";
import { usePagination } from "@/shared/hooks/use-pagination";
import { cn } from "@/shared/lib/utils";
import { Info } from "lucide-react";
import { useState } from "react";

const DEFAULT_CHILDREN_LIMIT = 3;

export function DepartmentTreeWidget() {
  const [departmentIds, setDepartmentIds] = useState<string[]>([]);
  const [excludedDepartmentIds, setExcludedDepartmentIds] = useState<string[]>(
    [],
  );
  const [locationIds, setLocationIds] = useState<string[]>([]);

  const { page, pageSize, onPageSizeChange } = usePagination(5);

  const [isActiveOnly, setIsActiveOnly] = useState<boolean>(false);

  const {
    departments,
    totalElements,
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
    isActiveOnly,
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

  return (
    <div className="flex gap-5">
      <div className="flex flex-col gap-2">
        <div className="flex gap-2">
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
          <Toggle
            size="sm"
            variant="outline"
            pressed={isActiveOnly}
            onPressedChange={setIsActiveOnly}
            className={cn("min-w-10", isActiveOnly && "w-25")}
          >
            <span>{isActiveOnly ? "Only active" : "All"}</span>
          </Toggle>
        </div>

        {departments && (
          <DepartmentTree
            departments={departments}
            selectedId={selectedId}
            onSelectedId={setSelectedId}
            isActiveOnly={isActiveOnly}
          />
        )}

        <LoadMoreButton
          pageSize={pageSize}
          totalElements={totalElements}
          onPageSizeChange={onPageSizeChange}
          loading={isDepartmentFetching}
        />
      </div>

      <div className="flex flex-col gap-3 w-[50%]">
        {!selectedId ? (
          <Alert className="bg-muted/40 w-fit pr-5 mx-auto">
            <Info className="h-4 w-4 text-muted-foreground" />
            <AlertTitle>Positions</AlertTitle>
            <AlertDescription className="text-muted-foreground">
              Select any department to see positions.
            </AlertDescription>
          </Alert>
        ) : isPositionFetching ? (
          <SkeletonCard quantity={5} layoutClassName="grid grid-cols-1 gap-2" />
        ) : isPositionError || (positionErrors && positionErrors.length > 0) ? (
          <ErrorCard errors={positionErrors ?? []} refetch={positionRefetch} />
        ) : positions?.length && departments?.length ? (
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
