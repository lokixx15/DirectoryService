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
  departmentIds?: string[]; 
  excludeDepartmentIds?: string[];
  isActive?: boolean;
  page: number;
  pageSize: number;
  orderBy?: "createdDate" | "updatedDate" | "name";
  orderDirection?: "asc" | "desc";
};
