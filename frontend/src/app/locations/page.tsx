"use client";

import { DataTable } from "./data-table";
import { columns } from "./columns";
import { SkeletonTable } from "@/shared/components/skeletons/skeleton-table";
import { usePagination } from "@/shared/hooks/use-pagination";
import { useLocationList } from "@/features/locations/model/useLocationList";

export default function Home() {
  const { page, pageSize, onPageIndexChange, onPageSizeChange } =
    usePagination();

  const { data, isPending } = useLocationList({ page, pageSize });

  if (isPending) {
    return <SkeletonTable />;
  }

  if (data?.isError) {
    return <div>Error: {data?.errorList?.[0]?.message ?? "Unknown error"}</div>;
  }

  return (
    <DataTable
      data={data?.result?.entities ?? []}
      columns={columns}
      page={page}
      pageSize={pageSize}
      totalPages={Math.ceil((data?.result?.totalCount ?? 1) / pageSize)}
      onPageIndexChange={onPageIndexChange}
      onPageSizeChange={onPageSizeChange}
    />
  );
}
