import { departmentsQueryOptions } from "@/entities/departments";
import { useMutation } from "@tanstack/react-query";

export const useDepartmentParentUpdate = () => {
  const mutation = useMutation(
    departmentsQueryOptions.updateDepartmentParent(),
  );

  return {
    updateParent: mutation.mutate,
  };
};
