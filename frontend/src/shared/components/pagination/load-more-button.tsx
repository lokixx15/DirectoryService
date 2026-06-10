import { LoaderCircle } from "lucide-react";
import { cn } from "@/shared/lib/utils";
import { Button } from "../ui/button";

const styles = `
@keyframes load-more-pulse {
  0%, 100% {
    opacity: 1;
    filter: brightness(1);
  }
  50% {
    opacity: 0.35;
    filter: brightness(0.7);
  }
}
.load-more-pulsing {
  animation: load-more-pulse 1.2s ease-in-out infinite;
}
`;

interface LoadMoreButtonProps {
  totalElements: number;
  pageSize: number;
  onPageSizeChange: (pageSize: number) => void;
  onPointerEnter?: () => void;
  className?: string;
  loading?: boolean;
}

export function LoadMoreButton({
  totalElements,
  pageSize,
  onPageSizeChange,
  onPointerEnter,
  className,
  loading,
}: LoadMoreButtonProps) {
  const handleClick = () => {
    if (!loading && totalElements && pageSize < totalElements) {
      const newPageSize = Math.min(pageSize + 10, totalElements);
      onPageSizeChange(newPageSize);
    }
  };

  return (
    <>
      {loading && <style>{styles}</style>}
      <Button
        variant="secondary"
        onPointerEnter={onPointerEnter}
        onClick={handleClick}
        disabled={loading}
        className={cn(className, loading && "load-more-pulsing")}
      >
        {loading && <LoaderCircle className="h-4 w-4 animate-spin" />}
        {loading ? "Загрузка..." : "Загрузить ещё"}
      </Button>
    </>
  );
}
