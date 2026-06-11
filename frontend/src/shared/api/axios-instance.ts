import axios from "axios";
import { EnvelopeErrors } from "./errors";
import { Envelope } from "./envelope";

export const apiClient = axios.create({
  baseURL: "http://localhost:5218/api",
  headers: {
    "Content-Type": "application/json",
  },
  paramsSerializer: {
    indexes: null,
  },
});

apiClient.interceptors.response.use(
  (response) => {
    const data = response.data as Envelope;

    if (data.isError && data.errorList) {
      throw new EnvelopeErrors(data.errorList);
    }

    return response;
  },
  (error) => {
    if (axios.isAxiosError(error) && error.response?.data) {
      const envelope = error.response?.data as Envelope;

      if (envelope.isError && envelope.errorList) {
        throw new EnvelopeErrors(envelope.errorList);
      }
    }

    return Promise.reject(error);
  },
);
