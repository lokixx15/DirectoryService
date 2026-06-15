export type Position = {
  id: string;
  name: string;
  description: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
  deletedAt?: Date;
};

export type GetPositionsRequest = {
  pageSize: number;
  cursor?: string;
  departmentIds?: string[];
  search?: string;
  isActive?: boolean;
};
