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
      .min(1, "Название локации обязательно.")
      .min(3, "Название локации должно содержать минимум 3 символа.")
      .max(120, "Название локации должно содержать не более 120 символов."),
    timezone: z
      .string()
      .min(1, "Часовой пояс обязателен.")
      .max(120, "Часовой пояс должен содержать не более 120 символов."),
    country: z.string().min(1, "Страна обязательна."),
    city: z.string().min(1, "Город обязателен."),
    street: z.string().min(1, "Улица обязательна."),
    building: z.string().min(1, "Здание обязательно."),
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
        message: `Полный адрес слишком длинный (${totalLength}/${MAX_ADDRESS_LENGTH} символов)`,
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
        <Button>Создать локацию</Button>
      </DialogTrigger>
      <DialogContent className="max-w-sm min-[850px]:max-w-200">
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogHeader className="mb-5">
            <DialogTitle>Создание локации</DialogTitle>
          </DialogHeader>
          {errors.root && (
            <div className="flex items-center gap-2 rounded-lg border border-destructive/40 bg-destructive/10 px-4 py-2.5 mb-4">
              <p className="text-sm text-destructive">{errors.root.message}</p>
            </div>
          )}
          <FieldGroup>
            <div className="grid grid-cols-1 min-[850px]:grid-cols-2 gap-5 max-w-sm mx-auto min-[850px]:max-w-none min-[850px]:mx-0">
              <FormInput
                label="Название"
                id="name"
                error={errors.name?.message}
                required={true}
                placeholder="Введите название..."
                {...register("name")}
              />
              <Controller
                name="timezone"
                control={control}
                render={({ field }) => (
                  <FormSelect
                    label="Часовой пояс"
                    id="timezone"
                    options={timezones.map((t) => ({
                      key: t.code,
                      value: t.code,
                      label: t.label,
                    }))}
                    error={errors.timezone?.message}
                    required
                    placeholder="Выберите часовой пояс"
                    value={field.value}
                    onChange={field.onChange}
                    onBlur={field.onBlur}
                    name={field.name}
                  />
                )}
              />
              <FormInput
                label="Страна"
                id="country"
                error={errors.country?.message}
                required={true}
                placeholder="Введите страну..."
                {...register("country")}
              />
              <FormInput
                label="Город"
                id="city"
                error={errors.city?.message}
                required={true}
                placeholder="Введите город..."
                {...register("city")}
              />
              <FormInput
                label="Улица"
                id="street"
                error={errors.street?.message}
                required={true}
                placeholder="Введите улицу..."
                {...register("street")}
              />
              <FormInput
                label="Здание"
                id="building"
                error={errors.building?.message}
                required={true}
                placeholder="Введите здание..."
                {...register("building")}
              />
              <FormInput
                label="Регион"
                id="region"
                error={errors.region?.message}
                placeholder="Введите регион..."
                {...register("region")}
              />
              <FormInput
                label="Район"
                id="district"
                error={errors.district?.message}
                placeholder="Введите район..."
                {...register("district")}
              />
              <FormInput
                label="Квартира"
                id="apartment"
                error={errors.apartment?.message}
                placeholder="Введите квартиру..."
                {...register("apartment")}
              />
            </div>
          </FieldGroup>
          <DialogFooter>
            <DialogClose asChild>
              <Button variant="outline" onClick={() => reset()}>
                Отмена
              </Button>
            </DialogClose>
            <Button type="submit" disabled={isPending}>
              Создать
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
