import { departmentsQueryOptions, DepartmentSummary } from "@/entities/departments";
import { useQuery } from "@tanstack/react-query";

interface UseDepartmentSummaryListReturn {
  departmentsSummary?: DepartmentSummary[];
  totalCount?: number;
  isLoading: boolean;
  isError: boolean;
}

interface UseDepartmentSummaryListProps {
  page: number;
  pageSize: number;
  search?: string;
}

export function useDepartmentSummaryList({
  page,
  pageSize,
  search,
}: UseDepartmentSummaryListProps): UseDepartmentSummaryListReturn {
  const { data, isLoading } = useQuery(
    departmentsQueryOptions.getDepartmentsSummaryOptions({
      page,
      pageSize,
      search: search || undefined,
    }),
  );

  return {
    departmentsSummary: data?.result?.entities,
    totalCount: data?.result?.totalCount,
    isLoading,
    isError: data?.isError ?? false,
  };
}
