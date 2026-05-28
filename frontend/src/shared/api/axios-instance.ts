import axios from "axios";

export const apiClient = axios.create({
  baseURL: "http://localhost:5218/api",
  headers: {
    "Content-Type": "application/json",
  },
  paramsSerializer: {
    indexes: null,
  },
});
