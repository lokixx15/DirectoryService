import { LocationAddress } from "./types";

export function formatAddress(address: LocationAddress): string {
  const parts = [
    address.country,
    address.city,
    address.street,
    address.building,
    address.region,
    address.district,
    address.apartment,
  ].filter((part) => part && part.trim());

  return parts.join(", ");
}
