import { locationsQueryOptions } from "@/entities/locations";
import { useMutation } from "@tanstack/react-query";

export function useDeleteLocation() {
  const mutation = useMutation(locationsQueryOptions.deleteLocation());

  return {
    deleteLocation: mutation.mutateAsync,
    isPending: mutation.isPending,
  };
}
