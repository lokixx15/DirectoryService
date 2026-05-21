import { locationsQueryOptions } from "@/entities/locations/api";
import { useQuery } from "@tanstack/react-query";

interface UseLocationListProps {
  page: number;
  pageSize: number;
}

export function useLocationList({ page, pageSize }: UseLocationListProps) {
  const { data, isPending } = useQuery(
    locationsQueryOptions.getLocationsOptions({ page, pageSize }),
  );

  return { data, isPending };
}
