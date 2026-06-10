"use client";

import { useState } from "react";
import { usePagination } from "@/shared/hooks/use-pagination";
import { useLocationList } from "@/features/locations";
import { DataTable, columns } from "@/widgets/locations";
import { SkeletonTable } from "@/shared/components/skeletons/skeleton-table";
import { ErrorCard } from "@/shared/components/errors/error-card";
import { SortingState } from "@tanstack/react-table";

export default function Home() {
  const { page, pageSize, onPageIndexChange, onPageSizeChange } =
    usePagination(10);
  const [search, setSearch] = useState("");
  const [sorting, setSorting] = useState<SortingState>([]);
  const [isActive, setIsActive] = useState<boolean>();
  const [departmentIds, setDepartmentIds] = useState<string[]>([]);

  const { locations, totalCount, totalPages, isPending, errors, refetch } = useLocationList(
    {
      page,
      pageSize,
      search,
      sorting,
      isActive,
      departmentIds,
    },
  );

  return (
    <div className="flex flex-col gap-2">
      {isPending ? (
        <SkeletonTable />
      ) : errors ? (
        <ErrorCard errors={errors} refetch={refetch} />
      ) : (
        <DataTable
          data={locations ?? []}
          columns={columns}
          page={page}
          pageSize={pageSize}
          totalCount={totalCount ?? 0}
          totalPages={totalPages ?? 1}
          onPageIndexChange={onPageIndexChange}
          onPageSizeChange={onPageSizeChange}
          sorting={sorting}
          onSortingChange={setSorting}
          onSearch={setSearch}
          onIsActive={setIsActive}
          onDepartmentIdsChange={setDepartmentIds}
        />
      )}
    </div>
  );
}
