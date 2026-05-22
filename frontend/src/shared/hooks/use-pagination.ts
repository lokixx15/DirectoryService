import { useState } from "react";

interface UsePaginationReturn {
  page: number;
  pageSize: number;
  onPageIndexChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

export function usePagination(): UsePaginationReturn {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);

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

