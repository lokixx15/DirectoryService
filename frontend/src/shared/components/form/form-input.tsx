import { Input } from "../ui/input";
import { FormField } from "./form-field";

interface FormInputProps {
  label: string;
  id?: string;
  error?: string;
  required?: boolean;
  placeholder?: string;
}

export function FormInput({
  label,
  id,
  required,
  error,
  placeholder,
  ...props
}: FormInputProps) {
  return (
    <FormField
      label={label}
      id={id}
      required={required}
      error={error}
    >
      <Input
        id={id}
        placeholder={placeholder}
        className={`w-full ${error ? "border-destructive focus-visible:ring-destructive" : ""}`}
        {...props}
      />
    </FormField>
  );
}
