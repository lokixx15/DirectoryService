import {
  infiniteQueryOptions,
  keepPreviousData,
  mutationOptions,
} from "@tanstack/react-query";
import { apiClient } from "@/shared/api/axios-instance";
import { Envelope } from "@/shared/api/envelope";
import { CursorPaginationResponse } from "@/shared/api/pagination-response";
import { GetPositionsRequest, Position } from "./types";
import { queryClient } from "@/shared/api/query-client";

export const positionsApi = {
  getAllPositions: async (request: GetPositionsRequest) => {
    const response = await apiClient.get<
      Envelope<CursorPaginationResponse<Position>>
    >("directory/positions", {
      params: request,
    });

    return response.data;
  },
  deletePosition: async (id: string) => {
    const response = await apiClient.delete<Envelope>(
      `directory/positions/${id}`,
    );

    return response.data;
  },
  restorePosition: async (id: string) => {
    const response = await apiClient.patch<Envelope>(
      `directory/positions/${id}/restore`,
    );

    return response.data;
  },
};

export const positionsQueryOptions = {
  baseKey: "positions",
  getAllPositionsInfinityQueryOptions: ({
    pageSize,
    departmentIds,
    search,
  }: GetPositionsRequest) => {
    return infiniteQueryOptions({
      queryKey: [positionsQueryOptions.baseKey, departmentIds, search],
      queryFn: async ({ pageParam }) => {
        return await positionsApi.getAllPositions({
          cursor: pageParam ?? undefined,
          departmentIds,
          search,
          pageSize,
        });
      },
      initialPageParam: null as string | null,
      placeholderData: keepPreviousData,
      getNextPageParam: (response) => {
        return response.result?.nextCursor ?? undefined;
      },
      select: (data): Envelope<CursorPaginationResponse<Position>> => {
        const lastPage = data.pages[data.pages.length - 1];
        return {
          result: {
            entities: data.pages.flatMap((p) => p.result?.entities ?? []),
            nextCursor: lastPage.result?.nextCursor ?? undefined,
          },
          errorList: data.pages.flatMap((p) => p.errorList ?? []),
          isError: data.pages[0].isError,
          timeGenerated: data.pages[0].timeGenerated,
        };
      },
    });
  },
  deletePositionOptions: () => {
    return mutationOptions({
      mutationFn: positionsApi.deletePosition,
      onSettled: () =>
        queryClient.invalidateQueries({
          queryKey: [positionsQueryOptions.baseKey],
        }),
    });
  },
  restoreLocationOptions: () => {
    return mutationOptions({
      mutationFn: positionsApi.restorePosition,
      onSettled: () =>
        queryClient.invalidateQueries({
          queryKey: [positionsQueryOptions.baseKey],
        }),
    });
  },
};
