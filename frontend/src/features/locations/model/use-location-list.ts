import { locationsQueryOptions, Location } from "@/entities/locations";
import { Error } from "@/shared/api/errors";
import { useQuery } from "@tanstack/react-query";
import { SortingState } from "@tanstack/react-table";

interface UseLocationListReturn {
  locations?: Location[];
  totalCount: number;
  totalPages: number;
  errors?: Error[] | null;
  isPending: boolean;
  refetch: () => void;
}

interface UseLocationListProps {
  page: number;
  pageSize: number;
  search: string;
  sorting?: SortingState;
  isActive?: boolean;
  selectedDepartmentIds: string[];
  excludedDepartmentIds: string[];
}

export function useLocationList({
  page,
  pageSize,
  search,
  sorting,
  isActive,
  selectedDepartmentIds,
  excludedDepartmentIds,
}: UseLocationListProps): UseLocationListReturn {
  const queryOptions = locationsQueryOptions.getLocationsOptions({
    page: page,
    pageSize: pageSize,
    search: search || undefined,
    orderBy: sorting?.[0]?.id,
    orderDirection: sorting?.[0]?.desc ? "desc" : "asc",
    isActive: isActive,
    selectedDepartmentIds:
      selectedDepartmentIds.length > 0 ? selectedDepartmentIds : undefined,
    excludedDepartmentIds:
      excludedDepartmentIds.length > 0 ? excludedDepartmentIds : undefined,
  });

  const { data, isPending, refetch } = useQuery(queryOptions);

  return {
    locations: data?.result?.entities,
    totalCount: data?.result?.totalCount || 0,
    totalPages: data?.result?.totalCount
      ? Math.ceil(data?.result?.totalCount / pageSize)
      : 0,
    errors: data?.errorList,
    isPending: isPending,
    refetch: refetch,
  };
}
