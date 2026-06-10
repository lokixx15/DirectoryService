import { Field, FieldLabel } from "../ui/field";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationNext,
  PaginationPrevious,
} from "../ui/pagination";
import {
  Select,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
  SelectContent,
} from "../ui/select";

interface PaginationIconsOnlyProps {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  canPreviousPage: boolean;
  canNextPage: boolean;
  onPageIndexChange: (pageIndex: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

export function PaginationIconsOnly({
  page,
  pageSize,
  totalCount,
  totalPages,
  canPreviousPage,
  canNextPage,
  onPageIndexChange,
  onPageSizeChange,
}: PaginationIconsOnlyProps) {
  return (
    <div className="flex items-center justify-between gap-4">
      <Field orientation="horizontal" className="w-fit">
        <FieldLabel htmlFor="select-rows-per-page">Строк на странице</FieldLabel>
        <Select
          value={pageSize.toString()}
          onValueChange={(value) => onPageSizeChange(Number(value))}
        >
          <SelectTrigger className="w-20" id="select-rows-per-page">
            <SelectValue />
          </SelectTrigger>
          <SelectContent align="start">
            <SelectGroup>
              <SelectItem value="10">10</SelectItem>
              <SelectItem value="20">20</SelectItem>
              <SelectItem value="25">25</SelectItem>
              <SelectItem value="50">50</SelectItem>
              <SelectItem value="100">100</SelectItem>
            </SelectGroup>
          </SelectContent>
        </Select>
      </Field>
      <div>
        <p className="text-sm">
          {page + 1} из {totalPages} страниц / {Math.min(pageSize * (page + 1), totalCount)} из {totalCount} элементов
        </p>
      </div>
      <Pagination className="mx-0 w-auto">
        <PaginationContent>
          <PaginationItem>
            <PaginationPrevious
              onClick={(e) => {
                e.preventDefault();
                if (canPreviousPage) onPageIndexChange(page - 1);
              }}
              href="#"
              aria-disabled={!canPreviousPage}
              className={!canPreviousPage ? "pointer-events-none opacity-50" : ""}
            />
          </PaginationItem>
          <PaginationItem>
            <PaginationNext
              onClick={(e) => {
                e.preventDefault();
                if (canNextPage) onPageIndexChange(page + 1);
              }}
              href="#"
              aria-disabled={!canNextPage}
              className={!canNextPage ? "pointer-events-none opacity-50" : ""}
            />
          </PaginationItem>
        </PaginationContent>
      </Pagination>
    </div>
  );
}
