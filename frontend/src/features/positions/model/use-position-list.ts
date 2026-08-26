import { useInfiniteQuery } from "@tanstack/react-query";
import { positionsQueryOptions } from "@/entities/positions/api";
import { Error } from "@/shared/api/errors";
import { Position } from "@/entities/positions/types";

interface UsePositionsListProps {
  pageSize: number;
  departmentIds?: string[];
  search?: string;
  isActive?: boolean;
  enabled?: boolean;
}

interface UsePositionsListReturn {
  positions?: Position[];
  errors?: Error[];
  isError: boolean;
  isFetching: boolean;
  isPending: boolean;
  isFetchingNextPage: boolean;
  hasNextPage: boolean;
  fetchNextPage: () => void;
  refetch: () => void;
}

export function usePositionList({
  pageSize,
  departmentIds,
  search,
  isActive,
  enabled,
}: UsePositionsListProps): UsePositionsListReturn {
  const {
    data,
    isFetching,
    isPending,
    isError,
    isFetchingNextPage,
    hasNextPage,
    fetchNextPage,
    refetch,
  } = useInfiniteQuery({
    ...positionsQueryOptions.getAllPositionsInfinityQueryOptions({
      pageSize,
      departmentIds: departmentIds?.length ? departmentIds : undefined,
      search: search || undefined,
      isActive: isActive,
    }),
    enabled,
  });

  return {
    positions: data?.result?.entities,
    errors: data?.errorList ?? undefined,
    isError: data?.isError || isError,
    isFetching,
    isPending,
    isFetchingNextPage,
    hasNextPage: hasNextPage ?? false,
    fetchNextPage,
    refetch,
  };
}
