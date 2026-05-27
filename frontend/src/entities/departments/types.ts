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
