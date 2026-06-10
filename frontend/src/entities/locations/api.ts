import { Envelope } from "@/shared/api/envelope";
import { CreateLocationRequest, GetLocationsRequest, Location } from "./types";
import { apiClient } from "@/shared/api/axios-instance";
import { PaginationResponse } from "@/shared/api/pagination-response";
import { queryClient } from "@/shared/api/query-client";
import {
  keepPreviousData,
  mutationOptions,
  queryOptions,
} from "@tanstack/react-query";
import { toast } from "sonner";
import { EnvelopeErrors } from "@/shared/api/errors";

export const locationsApi = {
  getLocations: async (request: GetLocationsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<Location>>
    >("/locations", {
      params: request,
    });

    return response.data;
  },
  createLocation: async (request: CreateLocationRequest) => {
    const response = await apiClient.post<Envelope<string>>(
      "/locations",
      request,
    );

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
      onError: (error) => {
        if (error instanceof EnvelopeErrors) {
          toast.error(error.firstMessage);
          return;
        }
        toast.error("Произошла ошибка при создании локации.");
      },
      onSuccess: () => {
        toast.success("Локация успешно создана");
      },
    });
  },
};
