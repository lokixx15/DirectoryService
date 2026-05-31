export type Location = {
  id: string;
  name: string;
  address: LocationAddress;
  timezone: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
};

export type LocationAddress = {
  country: string;
  city: string;
  street: string;
  building: string;
  region?: string | null;
  district?: string | null;
  apartment?: string | null;
};

export type GetLocationsRequest = {
  page: number;
  pageSize: number;
  departmentIds?: string[];
  search?: string;
  isActive?: boolean;
  orderBy?: string;
  orderDirection?: string;
};

export type CreateLocationRequest = {
  name: string;
  timezone: string;
  address: LocationAddress;
};
