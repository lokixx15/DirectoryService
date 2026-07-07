import { apiClient } from "@/shared/api/axios-instance";
import {
  DepartmentStandard,
  DepartmentSummary,
  GetDepartmentsRequest,
  GetDepartmentsSummaryRequest,
} from "./types";
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
  getDepartments: async (request: GetDepartmentsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<DepartmentStandard>>
    >("directory/departments", {
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
  getDepartmentsOptions: ({
    search,
    locationIds,
    departmentIds,
    excludeDepartmentIds,
    isActive,
    page,
    pageSize,
    orderBy,
    orderDirection,
  }: GetDepartmentsRequest) => {
    return queryOptions({
      queryFn: async () =>
        await departmentsApi.getDepartments({
          search,
          locationIds,
          departmentIds,
          excludeDepartmentIds,
          isActive,
          page: page + 1,
          pageSize,
          orderBy,
          orderDirection,
        }),
      queryKey: [
        departmentsQueryOptions.baseKey,
        search,
        locationIds,
        departmentIds,
        excludeDepartmentIds,
        isActive,
        page,
        pageSize,
        orderBy,
        orderDirection,
      ],
      placeholderData: keepPreviousData,
    });
  },
};
