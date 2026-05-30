import { useState, type ChangeEvent } from "react";
import { FormField } from "./form-field";

interface Option {
  key: string;
  value: string;
  label: string;
}

interface FormSelectProps {
  label: string;
  options: Option[];
  id?: string;
  error?: string;
  required?: boolean;
  placeholder?: string;
}

export function FormSelect({
  label,
  required,
  error,
  id,
  options,
  placeholder,
  ...props
}: FormSelectProps) {
  const [hasValue, setHasValue] = useState(false);

  const registerOnChange = (props as Record<string, unknown>)
    .onChange as ((e: ChangeEvent<HTMLSelectElement>) => void) | undefined;

  const handleChange = (e: ChangeEvent<HTMLSelectElement>) => {
    setHasValue(e.target.value !== "");
    registerOnChange?.(e);
  };

  return (
    <FormField label={label} required={required} error={error} id={id}>
      <select
        id={id}
        className={`w-full border rounded h-8 text-sm pl-1 ${!hasValue ? "text-muted-foreground" : ""}`}
        {...props}
        onChange={handleChange}
      >
        <option value="" disabled={hasValue} className={hasValue ? "hidden" : ""}>
          {placeholder}
        </option>
        {options.map((o) => (
          <option key={o.key} value={o.value}>
            {o.label}
          </option>
        ))}
      </select>
    </FormField>
  );
}
