import { Error } from "./errors";

export type Envelope<T = unknown> = {
  result: T | null;
  errorList: Error[] | null;
  isError: boolean;
  timeGenerated: string;
};
