import { Button } from "../ui/button";

interface LoadMoreButtonProps {
  totalElements: number;
  pageSize: number;
  onPageSizeChange: (pageSize: number) => void;
  onPointerEnter?: () => void;
  className?: string;
}

export function LoadMoreButton({
  totalElements,
  pageSize,
  onPageSizeChange,
  onPointerEnter,
  className,
}: LoadMoreButtonProps) {
  const handleClick = () => {
    if (totalElements && pageSize < totalElements) {
      const newPageSize = Math.min(pageSize + 10, totalElements);
      onPageSizeChange(newPageSize);
    }
  };

  return (
    <Button
      variant="secondary"
      onPointerEnter={onPointerEnter}
      onClick={handleClick}
      className={className}
    >
      Load more
    </Button>
  );
}
