import { useState } from "react";
import { EnvelopeErrors } from "./errors";

export type ServerErrors<T> = Partial<Record<keyof T, string>>;

export function useSetServerErrors<T>(validFields: Array<string>) {
  const [serverErrors, setServerErrors] = useState<ServerErrors<T>>({});
  const [formServerError, setFormServerError] = useState<string | null>(null);

  const applyEnvelopeErrors = (error: unknown) => {
    if (!(error instanceof EnvelopeErrors)) return;

    const fieldErrors: ServerErrors<T> = {};
    let formError: string | null = null;

    error.apiErrors.forEach((apiError) => {
      if (!apiError.message) return;

      if (apiError.invalidField && apiError.invalidField !== "Value") {
        const field = apiError.invalidField.toLowerCase();

        if (validFields.includes(field)) {
          fieldErrors[field as keyof T] = apiError.message;
        } else {
          formError = apiError.message;
        }
      } else {
        formError = apiError.message;
      }
    });

    setServerErrors(fieldErrors);
    if (formError) setFormServerError(formError);
  };

  const clearServerErrors = (field?: keyof T) => {
    if (field) {
      setServerErrors({ ...serverErrors, [field]: undefined });
    } else {
      setServerErrors({});
    }

    setFormServerError(null);
  };

  return {
    serverErrors,
    formServerError,
    applyEnvelopeErrors,
    clearServerErrors,
  };
}
