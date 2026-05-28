import { Location } from "./types";

export function formatAddress(address: Location["address"]): string {
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
