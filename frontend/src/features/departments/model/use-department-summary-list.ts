import { departmentsQueryOptions, DepartmentSummary } from "@/entities/departments";
import { useQuery } from "@tanstack/react-query";

interface UseDepartmentSummaryListReturn {
  departmentsSummary?: DepartmentSummary[];
  totalCount?: number;
  isFetching: boolean;
  isError: boolean;
  refetch: () => void;
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
  const { data, isFetching, isError, refetch } = useQuery(
    departmentsQueryOptions.getDepartmentsSummaryOptions({
      page,
      pageSize,
      search: search || undefined,
    }),
  );

  return {
    departmentsSummary: data?.result?.entities,
    totalCount: data?.result?.totalCount,
    isFetching,
    isError: data?.isError || isError,
    refetch,
  };
}
