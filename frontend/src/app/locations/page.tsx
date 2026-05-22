"use client";

import { useState } from "react";
import { usePagination } from "@/shared/hooks/use-pagination";
import { useLocationList } from "@/features/locations/model/use-location-list";
import { SearchBar } from "@/shared/components/search/search-bar";
import { DataTable } from "./data-table";
import { columns } from "./columns";
import { SkeletonTable } from "@/shared/components/skeletons/skeleton-table";
import { ErrorCard } from "@/shared/components/errors/error-card";

export default function Home() {
  const { page, pageSize, onPageIndexChange, onPageSizeChange } =
    usePagination();
  const [search, setSearch] = useState("");

  const { locations, totalCount, isPending, errors } = useLocationList({
    page,
    pageSize,
    search,
  });

  return (
    <div className="flex flex-col gap-2">
      <SearchBar onSearch={setSearch} />

      {isPending ? (
        <SkeletonTable />
      ) : errors ? (
        <ErrorCard errors={errors} />
      ) : (
        <DataTable
          data={locations ?? []}
          columns={columns}
          page={page}
          pageSize={pageSize}
          totalPages={Math.ceil((totalCount ?? 1) / pageSize)}
          onPageIndexChange={onPageIndexChange}
          onPageSizeChange={onPageSizeChange}
        />
      )}
    </div>
  );
}
