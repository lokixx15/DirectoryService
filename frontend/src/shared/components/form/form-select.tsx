import { FormField } from "./form-field";

interface Option {
  key: string;
  value: string;
  label: string;
}

interface FormSelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
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
  return (
    <FormField label={label} required={required} error={error} id={id}>
      <select
        id={id}
        required={required}
        className="w-full border rounded h-8 text-sm pl-1 invalid:text-muted-foreground"
        {...props}
      >
        <option value="" disabled>
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
