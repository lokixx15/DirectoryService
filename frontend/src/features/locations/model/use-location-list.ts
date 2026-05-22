import { locationsQueryOptions } from "@/entities/locations/api";
import { Location } from "@/entities/locations/types";
import { Error } from "@/shared/api/errors";
import { useQuery } from "@tanstack/react-query";

interface UseLocationListReturn {
  locations?: Location[];
  totalCount?: number;
  errors?: Error[] | null;
  isPending: boolean;
}

interface UseLocationListProps {
  page: number;
  pageSize: number;
  search: string;
}

export function useLocationList({
  page,
  pageSize,
  search,
}: UseLocationListProps): UseLocationListReturn {
  const { data, isPending } = useQuery(
    locationsQueryOptions.getLocationsOptions({ page, pageSize, search }),
  );

  console.log(data);

  return {
    locations: data?.result?.entities,
    totalCount: data?.result?.totalCount,
    errors: data?.errorList,
    isPending,
  };
}
