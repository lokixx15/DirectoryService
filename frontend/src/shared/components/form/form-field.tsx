import { ReactNode } from "react";
import { Field } from "../ui/field";
import { Label } from "../ui/label";

interface FormFieldProps {
  label: string;
  children: ReactNode;
  id?: string;
  error?: string;
  required?: boolean;
}

export function FormField({
  label,
  children,
  id,
  error,
  required,
}: FormFieldProps) {
  return (
    <Field>
      <Label htmlFor={id}>
        {label}
        {required && <span className="text-destructive">*</span>}
      </Label>
      {children}
      {error && <p className="text-[11px] text-destructive -mt-2.5">{error}</p>}
    </Field>
  );
}
