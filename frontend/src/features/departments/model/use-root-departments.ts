import { departmentsQueryOptions } from "@/entities/departments";
import { useQuery } from "@tanstack/react-query";

interface UseRootDepartments {
  page: number;
  size: number;
  prefetch: number;
  departmentIds: string[];
  excludedDepartmentIds: string[];
  isActiveOnly: boolean;
}

export function useRootDepartments({
  page,
  size,
  prefetch,
  departmentIds,
  excludedDepartmentIds,
  isActiveOnly,
}: UseRootDepartments) {
  const { data, isFetching, isPending, isError, refetch } = useQuery(
    departmentsQueryOptions.getRootDepartments({
      page,
      size,
      prefetch,
      departmentIds,
      excludedDepartmentIds,
      isActiveOnly,
    }),
  );

  return {
    departments: data?.result?.entities,
    totalElements: data?.result?.totalCount,
    isError: data?.isError || isError,
    errors: data?.errorList,
    isFetching,
    isPending,
    refetch,
  };
}
