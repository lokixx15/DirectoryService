import {
  departmentsQueryOptions,
  DepartmentSummary,
} from "@/entities/departments";
import { Error } from "@/shared/api/errors";
import { useQuery } from "@tanstack/react-query";

interface UseDepartmentSummaryListReturn {
  departmentsSummary?: DepartmentSummary[];
  totalCount?: number;
  isFetching: boolean;
  errors: Error[] | null | undefined;
  isPending: boolean;
  isError: boolean;
  refetch: () => void;
}

interface UseDepartmentSummaryListProps {
  page: number;
  pageSize: number;
  search?: string;
  excludedId?: string;
}

export function useDepartmentSummaryList({
  page,
  pageSize,
  search,
  excludedId,
}: UseDepartmentSummaryListProps): UseDepartmentSummaryListReturn {
  const { data, isFetching, isPending, isError, refetch } = useQuery(
    departmentsQueryOptions.getDepartmentsSummaryOptions({
      page,
      pageSize,
      search: search || undefined,
      excludedId,
    }),
  );

  return {
    departmentsSummary: data?.result?.entities,
    totalCount: data?.result?.totalCount,
    isPending,
    isFetching,
    errors: data?.errorList,
    isError: data?.isError || isError,
    refetch,
  };
}
