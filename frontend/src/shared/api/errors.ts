export type Envelope<T = unknown> = {
  result: T | null;
  errorList: Error[] | null;
  isError: boolean;
  timeGenerated: string;
};

export type Error = {
  code: string;
  message: string;
  errorType: "none" | "validation" | "not_found" | "failure" | "conflict";
  invalidField?: string;
};
