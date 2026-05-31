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
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useState } from "react";
import { useCreateLocation } from "../model/use-create-location";
import tzdb from "iana-db-timezones";
import { CreateLocationRequest } from "@/entities/locations";
import { FormInput } from "@/shared/components/form/form-input";
import { FormSelect } from "@/shared/components/form/form-select";

export function CreateLocationDialog() {
  const [open, setOpen] = useState(false);

  const createLocationSchema = z.object({
    name: z
      .string()
      .min(1, "Location name is required.")
      .min(3, "Location name must be at least 3 characters.")
      .max(120, "Location name should be no more than 120 characters."),
    timezone: z
      .string()
      .min(1, "Timezone is required.")
      .max(120, "Timezone should be no more than 120 characters."),
    country: z.string().min(1, "Country is required."),
    city: z.string().min(1, "City is required."),
    street: z.string().min(1, "Street is required."),
    building: z.string().min(1, "Building is required."),
    region: z.string().optional(),
    district: z.string().optional(),
    apartment: z.string().optional(),
  });

  type CreateLocationData = z.infer<typeof createLocationSchema>;

  const initialData: CreateLocationData = {
    name: "",
    timezone: "",
    country: "",
    city: "",
    street: "",
    building: "",
    region: "",
    district: "",
    apartment: "",
  };

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setError,
  } = useForm<CreateLocationData>({
    defaultValues: initialData,
    resolver: zodResolver(createLocationSchema),
  });

  const { createLocation, isPending } = useCreateLocation();

  const onSubmit = async (data: CreateLocationData) => {
    const addressParts = [
      data.country,
      data.city,
      data.street,
      data.building,
      data.region,
      data.district,
      data.apartment,
    ].filter(Boolean);

    const totalLength = addressParts.join(", ").length;
    const MAX_ADDRESS_LENGTH = 200;

    if (totalLength > MAX_ADDRESS_LENGTH) {
      setError("root", {
        message: `Full address is too long (${totalLength}/${MAX_ADDRESS_LENGTH} characters)`,
      });
      return;
    }

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

    createLocation(request, {
      onSuccess: () => {
        setOpen(false);
        reset();
      },
    });
  };

  const timezones = Object.entries(tzdb.zones).map(([code, zone]) => ({
    code: code,
    label: zone.label,
  }));

  return (
    <Dialog
      open={open}
      onOpenChange={(value) => {
        setOpen(value);
        reset();
      }}
    >
      <DialogTrigger asChild>
        <Button>Create location</Button>
      </DialogTrigger>
      <DialogContent className="max-w-sm min-[850px]:max-w-200">
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogHeader className="mb-5">
            <DialogTitle>Creating a location</DialogTitle>
          </DialogHeader>
          {errors.root && (
            <div className="flex items-center gap-2 rounded-lg border border-destructive/40 bg-destructive/10 px-4 py-2.5 mb-4">
              <p className="text-sm text-destructive">{errors.root.message}</p>
            </div>
          )}
          <FieldGroup>
            <div className="grid grid-cols-1 min-[850px]:grid-cols-2 gap-5 max-w-sm mx-auto min-[850px]:max-w-none min-[850px]:mx-0">
              <FormInput
                label="Name"
                id="name"
                error={errors.name?.message}
                required={true}
                placeholder="Enter the name..."
                {...register("name")}
              />
              <Controller
                name="timezone"
                control={control}
                render={({ field }) => (
                  <FormSelect
                    label="Timezone"
                    id="timezone"
                    options={timezones.map((t) => ({
                      key: t.code,
                      value: t.code,
                      label: t.label,
                    }))}
                    error={errors.timezone?.message}
                    required
                    placeholder="Select the timezone"
                    value={field.value}
                    onChange={field.onChange}
                    onBlur={field.onBlur}
                    name={field.name}
                  />
                )}
              />
              <FormInput
                label="Country"
                id="country"
                error={errors.country?.message}
                required={true}
                placeholder="Enter the country..."
                {...register("country")}
              />
              <FormInput
                label="City"
                id="city"
                error={errors.city?.message}
                required={true}
                placeholder="Enter the city..."
                {...register("city")}
              />
              <FormInput
                label="Street"
                id="street"
                error={errors.street?.message}
                required={true}
                placeholder="Enter the street..."
                {...register("street")}
              />
              <FormInput
                label="Building"
                id="building"
                error={errors.building?.message}
                required={true}
                placeholder="Enter the building..."
                {...register("building")}
              />
              <FormInput
                label="Region"
                id="region"
                error={errors.region?.message}
                placeholder="Enter the region..."
                {...register("region")}
              />
              <FormInput
                label="District"
                id="district"
                error={errors.district?.message}
                placeholder="Enter the district..."
                {...register("district")}
              />
              <FormInput
                label="Apartment"
                id="apartment"
                error={errors.apartment?.message}
                placeholder="Enter the apartment..."
                {...register("apartment")}
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
              Submit
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
