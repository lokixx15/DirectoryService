import { Envelope } from "@/shared/api/envelope";
import {
  CreateLocationRequest,
  EditLocationRequest,
  GetLocationsRequest,
  Location,
} from "./types";
import { apiClient } from "@/shared/api/axios-instance";
import { PaginationResponse } from "@/shared/api/pagination-response";
import { queryClient } from "@/shared/api/query-client";
import { keepPreviousData, mutationOptions, queryOptions } from "@tanstack/react-query";

export const locationsApi = {
  getLocations: async (request: GetLocationsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<Location>>
    >("directory/locations", {
      params: request,
    });

    return response.data;
  },
  createLocation: async (request: CreateLocationRequest) => {
    const response = await apiClient.post<Envelope<string>>(
      "directory/locations",
      request,
    );

    return response.data;
  },
  editLocation: async (request: EditLocationRequest) => {
    const response = await apiClient.put<Envelope>(
      `directory/locations/${request.id}`,
      request,
    );

    return response.data;
  },
  deleteLocation: async (id: string) => {
    const response = await apiClient.delete<Envelope>(`directory/locations/${id}`);

    return response.data;
  },
};

export const locationsQueryOptions = {
  baseKey: "locations",
  getLocationsOptions: ({
    page,
    pageSize,
    search,
    orderBy,
    orderDirection,
    isActive,
    departmentIds,
  }: GetLocationsRequest) => {
    return queryOptions({
      queryFn: async () =>
        await locationsApi.getLocations({
          page: page + 1,
          pageSize,
          search,
          orderBy,
          orderDirection,
          isActive,
          departmentIds,
        }),
      queryKey: [
        locationsQueryOptions.baseKey,
        page,
        pageSize,
        search,
        orderBy,
        orderDirection,
        isActive,
        departmentIds,
      ],
      placeholderData: keepPreviousData,
    });
  },
  createLocationOptions: () => {
    return mutationOptions({
      mutationFn: locationsApi.createLocation,
      onSettled: () =>
        queryClient.invalidateQueries({
          queryKey: [locationsQueryOptions.baseKey],
        }),
    });
  },
  editLocationOptions: () => {
    return mutationOptions({
      mutationFn: locationsApi.editLocation,
      onSettled: () =>
        queryClient.invalidateQueries({
          queryKey: [locationsQueryOptions.baseKey],
        }),
    });
  },
  deleteLocation: () => {
    return mutationOptions({
      mutationFn: locationsApi.deleteLocation,
      onSettled: () =>
        queryClient.invalidateQueries({
          queryKey: [locationsQueryOptions.baseKey],
        }),
    });
  },
};
