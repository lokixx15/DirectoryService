export type OrderBy = "createdDate" | "updatedDate" | "name" | undefined;
export type OrderDirection = "asc" | "desc" | undefined;
export type OrderState = {
  orderBy: OrderBy;
  orderDirection: OrderDirection;
};
