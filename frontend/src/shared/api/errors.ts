export type Error = {
  code: string;
  message: string;
  type: ErrorType;
  invalidField?: string;
};

export type ErrorType =
  | "none"
  | "validation"
  | "not_found"
  | "failure"
  | "conflict";

export class EnvelopeErrors extends Error {
  public readonly apiErrors: Error[];

  constructor(apiErrors: Error[]) {
    const firstMessage = apiErrors[0].message ?? "Unknown error";
    super(firstMessage);
    this.apiErrors = apiErrors;

    Object.setPrototypeOf(this, EnvelopeErrors.prototype);
  }

  get errors(): Error[] {
    return this.apiErrors;
  }

  get firstError(): Error {
    return this.apiErrors[0];
  }

  get allMessages(): string[] {
    return this.apiErrors.map((error) => error.message);
  }

  get firstMessage(): string {
    return this.apiErrors[0].message;
  }

  firstMessageByInvalidField(invalidField: string): string | undefined {
    return this.apiErrors.find(
      (error) =>
        error.invalidField?.toLocaleLowerCase() ===
        invalidField.toLocaleLowerCase(),
    )?.message;
  }
}

export function isEnvelopeError(error: unknown): error is EnvelopeErrors {
  return error instanceof EnvelopeErrors;
}

