import { departmentsQueryOptions } from "@/entities/departments";
import { OrderState } from "@/shared/types/ordering";
import { useQuery } from "@tanstack/react-query";

interface UseDepartmentsProps {
  search?: string;
  locationIds?: string[];
  isActive?: boolean;
  page: number;
  pageSize: number;
  order?: OrderState;
}

export function useDepartments({
  search,
  locationIds,
  isActive,
  page,
  pageSize,
  order,
}: UseDepartmentsProps) {
  const { data, isFetching, isPending, isError, refetch } = useQuery(
    departmentsQueryOptions.getDepartmentsOptions({
      search: search || undefined,
      locationIds: locationIds || undefined,
      isActive: isActive,
      page: page,
      pageSize: pageSize,
      orderBy: order?.orderBy ?? undefined,
      orderDirection: order?.orderDirection ?? undefined,
    }),
  );

  return {
    departments: data?.result?.entities,
    isError: data?.isError || isError,
    totalCount: data?.result?.totalCount || 0,
    totalPages: data?.result?.totalCount
      ? Math.ceil(data?.result?.totalCount / pageSize)
      : 0,
    errors: data?.errorList,
    isFetching,
    isPending,
    refetch,
  };
}
