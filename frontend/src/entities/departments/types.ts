export type DepartmentSummary = {
  id: string;
  name: string;
  identifier: string;
};

export type GetDepartmentsSummaryRequest = {
  page: number;
  pageSize: number;
  search?: string;
};

export type DepartmentStandard = {
  id: string;
  name: string;
  identifier: string;
  path: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
  deletedAt: Date;
};

export type GetDepartmentsRequest = {
  search?: string;
  locationIds?: string[];
  isActive?: boolean;
  page: number;
  pageSize: number;
  orderBy?: "createdDate" | "updatedDate" | "name";
  orderDirection?: "asc" | "desc";
};

export type Department = {
  id: string;
  name: string;
  identifier: string;
  parentId: string | null;
  path: string;
  depth: number;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
  hasMoreChildren: boolean;
};

export type GetRootDepartmentsRequest = {
  page: number;
  size: number;
  prefetch: number;
  departmentIds: string[];
  excludedDepartmentIds: string[];
};

export type GetChildrenDepartmentsRequest = {
  page: number;
  size: number;
  parentId: string;
};
