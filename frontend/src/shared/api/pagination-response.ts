export type PaginationResponse<T> = {
  entities: T[];
  totalCount: number;
};

export type CursorPaginationResponse<T> = {
  entities: T[];
  nextCursor?: string | null;
};
