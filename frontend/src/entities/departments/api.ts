import { apiClient } from "@/shared/api/axios-instance";
import {
  Department,
  DepartmentStandard,
  DepartmentSummary,
  GetChildrenDepartmentsRequest,
  GetDepartmentsRequest,
  GetDepartmentsSummaryRequest,
  GetRootDepartmentsRequest,
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
  getRootDepartments: async (request: GetRootDepartmentsRequest) => {
    const response = await apiClient.get<Envelope<Department[]>>(
      "directory/departments/roots",
      {
        params: request,
      },
    );

    return response.data;
  },
  getChildrenDepartments: async (request: GetChildrenDepartmentsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<Department>>
    >(`directory/departments/${request.parentId}/children`, {
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
        isActive,
        page,
        pageSize,
        orderBy,
        orderDirection,
      ],
      placeholderData: keepPreviousData,
    });
  },
  getRootDepartments: ({
    page,
    size,
    prefetch,
    departmentIds,
    excludedDepartmentIds,
  }: GetRootDepartmentsRequest) => {
    return queryOptions({
      queryFn: async () =>
        await departmentsApi.getRootDepartments({
          page: page + 1,
          size,
          prefetch,
          departmentIds,
          excludedDepartmentIds,
        }),
      queryKey: [
        departmentsQueryOptions.baseKey,
        page,
        size,
        prefetch,
        departmentIds,
        excludedDepartmentIds,
      ],
      placeholderData: keepPreviousData,
    });
  },
  getChildrenDepartments: (
    { page, size, parentId }: GetChildrenDepartmentsRequest,
  ) => {
    return queryOptions({
      queryFn: async () =>
        await departmentsApi.getChildrenDepartments({
          page: page + 1,
          size,
          parentId,
        }),
      queryKey: [departmentsQueryOptions.baseKey, page, size, parentId],
      placeholderData: keepPreviousData,
    });
  },
};
