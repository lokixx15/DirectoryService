import axios from "axios";
import { GetLocationsRequest, Location } from "@/entities/locations/types";
import { apiClient } from "@/shared/api/axios-instance";
import { Envelope } from "@/shared/api/errors";
import { PaginationResponse } from "@/shared/api/pagination-response";
import { keepPreviousData, queryOptions } from "@tanstack/react-query";

export const locationsApi = {
  getLocations: async (request: GetLocationsRequest) => {
    try {
      const response = await apiClient.get<
        Envelope<PaginationResponse<Location>>
      >("/locations", {
        params: request,
      });

      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.data) {
        return error.response.data as Envelope<PaginationResponse<Location>>;
      }
      throw error;
    }
  },
};

export const locationsQueryOptions = {
  baseKey: "locations",

  getLocationsOptions: ({ page, pageSize, search }: GetLocationsRequest) => {
    return queryOptions({
      queryFn: async () =>
        await locationsApi.getLocations({
          page: page + 1,
          pageSize: pageSize,
          search: search,
        }),
      queryKey: ["locations", page, pageSize, search],
      placeholderData: keepPreviousData,
    });
  },
};
