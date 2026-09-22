import { positionsQueryOptions } from "@/entities/positions/api";
import { useMutation } from "@tanstack/react-query";

export function usePositionDelete() {
  const mutation = useMutation(positionsQueryOptions.deletePositionOptions());

  return {
    deletePosition: mutation.mutate,
    isPending: mutation.isPending,
  };
}
