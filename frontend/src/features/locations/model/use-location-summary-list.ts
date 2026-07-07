import { locationsQueryOptions } from "@/entities/locations";
import { LocationSummary } from "@/entities/locations/types";
import { useQuery } from "@tanstack/react-query";

interface UseLocationSummaryListReturn {
  locationsSummary?: LocationSummary[];
  totalCount?: number;
  isFetching: boolean;
  isError: boolean;
  refetch: () => void;
}

interface UseLocationSummaryListProps {
  page: number;
  pageSize: number;
  search?: string;
}

export function useLocationSummaryList({
  page,
  pageSize,
  search,
}: UseLocationSummaryListProps): UseLocationSummaryListReturn {
  const { data, isFetching, isError, refetch } = useQuery(
    locationsQueryOptions.getLocationsSummaryOptions({
      page,
      pageSize,
      search: search || undefined,
    }),
  );

  return {
    locationsSummary: data?.result?.entities,
    totalCount: data?.result?.totalCount,
    isFetching,
    isError: data?.isError || isError,
    refetch,
  };
}
