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
        message: `Полный адрес слишком длинный (${fullAddress.length}/${MAX_ADDRESS_LENGTH} символов)`,
      });
      return;
    }

    createLocation(request, {
      onSuccess: () => {
        toast.success("Локация успешно создана");
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
          toast.error("Ошибка при создании локации");
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
        <Button>Создать локацию</Button>
      </DialogTrigger>
      <DialogContent className="max-w-sm min-[850px]:max-w-200">
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogHeader className="mb-5">
            <DialogTitle>Создание локации</DialogTitle>
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
                label="Название"
                id="name"
                error={errors.name?.message || serverErrors.name}
                required={true}
                placeholder="Введите название..."
                {...register("name", {
                  onChange: () => {
                    if (serverErrors.name) {
                      clearServerErrors("name");
                    }
                  },
                })}
              />
              <FormSelect
                label="Часовой пояс"
                id="timezone"
                options={timezones.map((t) => ({
                  key: t.code,
                  value: t.code,
                  label: t.label,
                }))}
                error={errors.timezone?.message || serverErrors.timezone}
                required
                placeholder="Выберите часовой пояс"
                {...register("timezone", {
                  onChange: () => {
                    if (serverErrors.name) {
                      clearServerErrors("timezone");
                    }
                  },
                })}
              />
              <FormInput
                label="Страна"
                id="country"
                error={errors.country?.message || serverErrors.country}
                required={true}
                placeholder="Введите страну..."
                {...register("country", {
                  onChange: () => {
                    if (serverErrors.name) {
                      clearServerErrors("country");
                    }
                  },
                })}
              />
              <FormInput
                label="Город"
                id="city"
                error={errors.city?.message || serverErrors.city}
                required={true}
                placeholder="Введите город..."
                {...register("city", {
                  onChange: () => {
                    if (serverErrors.name) {
                      clearServerErrors("city");
                    }
                  },
                })}
              />
              <FormInput
                label="Улица"
                id="street"
                error={errors.street?.message || serverErrors.street}
                required={true}
                placeholder="Введите улицу..."
                {...register("street", {
                  onChange: () => {
                    if (serverErrors.name) {
                      clearServerErrors("street");
                    }
                  },
                })}
              />
              <FormInput
                label="Здание"
                id="building"
                error={errors.building?.message || serverErrors.building}
                required={true}
                placeholder="Введите здание..."
                {...register("building", {
                  onChange: () => {
                    if (serverErrors.name) {
                      clearServerErrors("building");
                    }
                  },
                })}
              />
              <FormInput
                label="Регион"
                id="region"
                error={errors.region?.message || serverErrors.region}
                placeholder="Введите регион..."
                {...register("region", {
                  onChange: () => {
                    if (serverErrors.name) {
                      clearServerErrors("region");
                    }
                  },
                })}
              />
              <FormInput
                label="Район"
                id="district"
                error={errors.district?.message || serverErrors.district}
                placeholder="Введите район..."
                {...register("district", {
                  onChange: () => {
                    if (serverErrors.name) {
                      clearServerErrors("district");
                    }
                  },
                })}
              />
              <FormInput
                label="Квартира"
                id="apartment"
                error={errors.apartment?.message || serverErrors.apartment}
                placeholder="Введите квартиру..."
                {...register("apartment", {
                  onChange: () => {
                    if (serverErrors.name) {
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
