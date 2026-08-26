import { departmentsQueryOptions } from "@/entities/departments";
import { useQuery } from "@tanstack/react-query";

interface UseRootDepartments {
  page: number;
  size: number;
  prefetch: number;
  departmentIds: string[];
  excludedDepartmentIds: string[];
}

export function useRootDepartments({
  page,
  size,
  prefetch,
  departmentIds,
  excludedDepartmentIds,
}: UseRootDepartments) {
  const { data, isFetching, isPending, isError, refetch } = useQuery(
    departmentsQueryOptions.getRootDepartments({
      page,
      size,
      prefetch,
      departmentIds,
      excludedDepartmentIds,
    }),
  );

  return {
    departments: data?.result,
    isError: data?.isError || isError,
    errors: data?.errorList,
    isFetching,
    isPending,
    refetch,
  };
}
