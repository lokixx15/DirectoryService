import { locationsQueryOptions } from "@/entities/locations";
import { useMutation } from "@tanstack/react-query";

export function useEditLocation() {
  const mutation = useMutation(locationsQueryOptions.editLocationOptions());

  return {
    editLocation: mutation.mutateAsync,
    isPending: mutation.isPending,
  };
}
