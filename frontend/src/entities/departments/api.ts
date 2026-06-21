import { apiClient } from "@/shared/api/axios-instance";
import { DepartmentSummary, GetDepartmentsSummaryRequest } from "./types";
import { Envelope } from "@/shared/api/envelope";
import { PaginationResponse } from "@/shared/api/pagination-response";
import { keepPreviousData, queryOptions } from "@tanstack/react-query";

export const departmentsApi = {
  getDepartmentsSummary: async (request: GetDepartmentsSummaryRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<DepartmentSummary>>
    >("directory/departments/summary", {
      params: request,
    });

    return response.data;
  },
};

export const departmentsQueryOptions = {
  baseKey: "departments",
  getDepartmentsSummaryOptions: ({
    page,
    pageSize,
    search,
  }: GetDepartmentsSummaryRequest) => {
    return queryOptions({
      queryFn: async () =>
        await departmentsApi.getDepartmentsSummary({
          page: page + 1,
          pageSize,
          search,
        }),
      queryKey: [
        departmentsQueryOptions.baseKey,
        "summary",
        page,
        pageSize,
        search,
      ],
      placeholderData: keepPreviousData,
    });
  },
};
