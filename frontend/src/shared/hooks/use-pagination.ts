import { useState } from "react";

interface UsePaginationReturn {
  page: number;
  pageSize: number;
  onPageIndexChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

export function usePagination(defaultPageSize: number): UsePaginationReturn {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(defaultPageSize);

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    setPage(0);
  };

  return {
    page,
    pageSize,
    onPageIndexChange: setPage,
    onPageSizeChange: handlePageSizeChange,
  };
}
