import { departmentsQueryOptions } from "@/entities/departments";
import { useMutation } from "@tanstack/react-query";

export function useDepartmentDelete() {
  const mutation = useMutation(
    departmentsQueryOptions.deleteDepartmentOptions(),
  );

  return {
    deleteDepartment: mutation.mutate,
    isPending: mutation.isPending,
  };
}
