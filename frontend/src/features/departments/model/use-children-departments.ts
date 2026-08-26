import { departmentsQueryOptions } from "@/entities/departments";
import { useQuery } from "@tanstack/react-query";

interface UseChildrenDepartments {
  page: number;
  size: number;
  parentId: string;
  enabled?: boolean;
}

export function useChildrenDepartments({
  page,
  size,
  parentId,
  enabled,
}: UseChildrenDepartments) {
  const { data, isFetching, isError, refetch } = useQuery({
    ...departmentsQueryOptions.getChildrenDepartments({
      page,
      size,
      parentId,
    }),
    enabled: enabled && size > 0 && !!parentId,
  });

  return {
    nestedChildrenDepartments: data?.result?.entities,
    totalCount: data?.result?.totalCount,
    isError: data?.isError || isError,
    errors: data?.errorList,
    isFetching,
    refetch,
  };
}
