import { apiClient } from "@/shared/api/axios-instance";
import { DepartmentSummary, GetDepartmentsSummaryRequest } from "./types";
import { Envelope } from "@/shared/api/errors";
import { PaginationResponse } from "@/shared/api/pagination-response";
import axios from "axios";
import { keepPreviousData, queryOptions } from "@tanstack/react-query";

export const departmentsApi = {
  getDepartmentsSummary: async (request: GetDepartmentsSummaryRequest) => {
    try {
      const response = await apiClient.get<
        Envelope<PaginationResponse<DepartmentSummary>>
      >("/departments/summary", {
        params: request,
      });

      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.data) {
        return error.response.data as Envelope<
          PaginationResponse<DepartmentSummary>
        >;
      }
      throw error;
    }
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
      queryKey: [departmentsQueryOptions.baseKey, "summary", page, pageSize, search],
      placeholderData: keepPreviousData,
    });
  },
};
