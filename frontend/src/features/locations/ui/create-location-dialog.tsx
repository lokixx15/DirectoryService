"use client";

import { Button } from "@/shared/components/ui/button";
import {
  Dialog,
  DialogTrigger,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogClose,
} from "@/shared/components/ui/dialog";
import { FieldGroup } from "@/shared/components/ui/field";
import { z } from "zod";
import { FieldPath, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useState } from "react";
import { useCreateLocation } from "../model/use-create-location";
import tzdb from "iana-db-timezones";
import { CreateLocationRequest, formatAddress } from "@/entities/locations";
import { FormInput } from "@/shared/components/form/form-input";
import { FormSelect } from "@/shared/components/form/form-select";
import { useSetServerErrors } from "@/shared/api/form-errors";
import { toast } from "sonner";
import { isEnvelopeError } from "@/shared/api/errors";

const MAX_ADDRESS_LENGTH = 200;

const createLocationSchema = z.object({
  name: z
    .string()
    .min(1, "Location name is required.")
    .min(3, "Location name must be at least 3 characters.")
    .max(120, "Location name must not exceed 120 characters."),
  timezone: z
    .string()
    .min(1, "Timezone is required.")
    .max(120, "Timezone must not exceed 120 characters."),
  country: z.string().min(1, "Country is required."),
  city: z.string().min(1, "City is required."),
  street: z.string().min(1, "Street is required."),
  building: z.string().min(1, "Building is required."),
  region: z.string().optional(),
  district: z.string().optional(),
  apartment: z.string().optional(),
});

type CreateLocationData = z.infer<typeof createLocationSchema>;

export function CreateLocationDialog() {
  const [open, setOpen] = useState(false);

  const {
    setError,
    register,
    handleSubmit,
    formState: { errors },
    reset,
    clearErrors,
  } = useForm<CreateLocationData>({
    defaultValues: {
      name: "",
      timezone: "",
      country: "",
      city: "",
      street: "",
      building: "",
      region: "",
      district: "",
      apartment: "",
    },
    resolver: zodResolver(createLocationSchema),
  });

  const { createLocation, isPending } = useCreateLocation();
  const {
    serverErrors,
    formServerError,
    applyEnvelopeErrors,
    clearServerErrors,
  } = useSetServerErrors<CreateLocationData>([
    "name",
    "timezone",
    "country",
    "city",
    "street",
    "building",
    "region",
    "district",
    "apartment",
  ]);

  const clearAllErrors = () => {
    clearServerErrors();
    clearErrors();
  };

  const onDialogChange = (open: boolean) => {
    setOpen(open);
    clearAllErrors();
    reset();
  };

  const onSubmit = async (data: CreateLocationData) => {
    clearErrors();

    const request: CreateLocationRequest = {
      name: data.name,
      timezone: data.timezone,
      address: {
        country: data.country,
        city: data.city,
        street: data.street,
        building: data.building,
        region: data.region,
        district: data.district,
        apartment: data.apartment,
      },
    };

    const fullAddress = formatAddress({
      country: data.country,
      city: data.city,
      street: data.street,
      building: data.building,
      region: data.region,
      district: data.district,
      apartment: data.apartment,
    });

    if (fullAddress.length > MAX_ADDRESS_LENGTH) {
      setError("root" as FieldPath<CreateLocationData>, {
        type: "manual",
        message: `Full address is too long (${fullAddress.length}/${MAX_ADDRESS_LENGTH} characters)`,
      });
      return;
    }

    createLocation(request, {
      onSuccess: () => {
        toast.success("Location created successfully");
        setOpen(false);
        reset();
        clearServerErrors();
      },
      onError: (errors) => {
        if (isEnvelopeError(errors)) {
          applyEnvelopeErrors(errors);
          errors.apiErrors.forEach((error) => {
            toast.error(error.message);
          });
        } else {
          toast.error("Error creating location");
        }
      },
    });
  };

  const timezones = Object.entries(tzdb.zones).map(([code, zone]) => ({
    code: code,
    label: zone.label,
  }));

  return (
    <Dialog open={open} onOpenChange={onDialogChange}>
      <DialogTrigger asChild>
        <Button>Create location</Button>
      </DialogTrigger>
      <DialogContent className="max-w-sm min-[850px]:max-w-200">
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogHeader className="mb-5">
            <DialogTitle>Create location</DialogTitle>
          </DialogHeader>
          {(errors.root || formServerError) && (
            <div className="flex items-center gap-2 rounded-lg border border-destructive/40 bg-destructive/10 px-4 py-2.5 mb-4">
              <p className="text-sm text-destructive">
                {errors.root?.message || formServerError}
              </p>
            </div>
          )}
          <FieldGroup>
            <div className="grid grid-cols-1 min-[850px]:grid-cols-2 gap-5 max-w-sm mx-auto min-[850px]:max-w-none min-[850px]:mx-0">
              <FormInput
                label="Name"
                id="name"
                error={errors.name?.message || serverErrors.name}
                required={true}
                placeholder="Search..."
                {...register("name", {
                  onChange: () => {
                    if (serverErrors.name) {
                      clearServerErrors("name");
                    }
                  },
                })}
              />
              <FormSelect
                label="Timezone"
                id="timezone"
                options={timezones.map((t) => ({
                  key: t.code,
                  value: t.code,
                  label: t.label,
                }))}
                error={errors.timezone?.message || serverErrors.timezone}
                required
                placeholder="Select timezone"
                {...register("timezone", {
                  onChange: () => {
                    if (serverErrors.timezone) {
                      clearServerErrors("timezone");
                    }
                  },
                })}
              />
              <FormInput
                label="Country"
                id="country"
                error={errors.country?.message || serverErrors.country}
                required={true}
                placeholder="Enter country..."
                {...register("country", {
                  onChange: () => {
                    if (serverErrors.country) {
                      clearServerErrors("country");
                    }
                  },
                })}
              />
              <FormInput
                label="City"
                id="city"
                error={errors.city?.message || serverErrors.city}
                required={true}
                placeholder="Enter city..."
                {...register("city", {
                  onChange: () => {
                    if (serverErrors.city) {
                      clearServerErrors("city");
                    }
                  },
                })}
              />
              <FormInput
                label="Street"
                id="street"
                error={errors.street?.message || serverErrors.street}
                required={true}
                placeholder="Enter street..."
                {...register("street", {
                  onChange: () => {
                    if (serverErrors.street) {
                      clearServerErrors("street");
                    }
                  },
                })}
              />
              <FormInput
                label="Building"
                id="building"
                error={errors.building?.message || serverErrors.building}
                required={true}
                placeholder="Enter building..."
                {...register("building", {
                  onChange: () => {
                    if (serverErrors.building) {
                      clearServerErrors("building");
                    }
                  },
                })}
              />
              <FormInput
                label="Region"
                id="region"
                error={errors.region?.message || serverErrors.region}
                placeholder="Enter region..."
                {...register("region", {
                  onChange: () => {
                    if (serverErrors.region) {
                      clearServerErrors("region");
                    }
                  },
                })}
              />
              <FormInput
                label="District"
                id="district"
                error={errors.district?.message || serverErrors.district}
                placeholder="Enter district..."
                {...register("district", {
                  onChange: () => {
                    if (serverErrors.district) {
                      clearServerErrors("district");
                    }
                  },
                })}
              />
              <FormInput
                label="Apartment"
                id="apartment"
                error={errors.apartment?.message || serverErrors.apartment}
                placeholder="Enter apartment..."
                {...register("apartment", {
                  onChange: () => {
                    if (serverErrors.apartment) {
                      clearServerErrors("apartment");
                    }
                  },
                })}
              />
            </div>
          </FieldGroup>
          <DialogFooter>
            <DialogClose asChild>
              <Button variant="outline" onClick={() => reset()}>
                Cancel
              </Button>
            </DialogClose>
            <Button type="submit" disabled={isPending}>
              Create
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
