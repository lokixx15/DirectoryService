import { GetLocationsRequest, Location } from "@/entities/locations/types";
import { apiClient } from "@/shared/api/axios-instance";
import { Envelope } from "@/shared/api/errors";
import { PaginationResponse } from "@/shared/api/pagination-response";
import { queryOptions } from "@tanstack/react-query";

export const locationsApi = {
  getLocations: async (request: GetLocationsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<Location>>
    >("/locations", {
      params: request,
    });

    return response.data;
  },
};

export const locationsQueryOptions = {
  baseKey: "locations",

  getLocationsOptions: ({ page, pageSize }: GetLocationsRequest) => {
    return queryOptions({
      queryFn: async () =>
        await locationsApi.getLocations({
          page: page + 1,
          pageSize: pageSize,
        }),
      queryKey: ["locations", page, pageSize],
    });
  },
};
