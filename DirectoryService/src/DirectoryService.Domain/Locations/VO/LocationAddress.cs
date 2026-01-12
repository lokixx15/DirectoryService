using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using SharedKernel;
using System.Diagnostics.Metrics;
using System.IO;
using System.Text.Json.Serialization;

namespace DirectoryService.Domain.Locations.VO;

public record LocationAddress
{
    //ef core
    private LocationAddress() { }

    private LocationAddress(
        string country,
        string city,
        string street,
        string building,
        string? region,
        string? district,
        string? apartment)
    {
        Country = country;
        City = city;
        Street = street;
        Building = building;
        Region = region;
        District = district;
        Apartment = apartment;
    }

    public const string SEPARATOR = ", ";

    public string Country { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Street { get; private set; } = string.Empty;
    public string Building { get; private set; } = string.Empty;
    public string? Region { get; private set; } = string.Empty;
    public string? District { get; private set; } = string.Empty;
    public string? Apartment { get; private set; } = string.Empty;
    public string FullAddress => GetFullAddress(Country, City, Street, Building, Region, District, Apartment);

    public static Result<LocationAddress, Errors> Create(
        string country,
        string city,
        string street,
        string building,
        string? region = null,
        string? district = null,
        string? apartment = null)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(country))
            return Result.Failure<LocationAddress, Errors>(GeneralErrors.ValueIsNullOrWhitespace("Country"));

        if (string.IsNullOrWhiteSpace(city))
            return Result.Failure<LocationAddress, Errors>(GeneralErrors.ValueIsNullOrWhitespace("City"));

        if (string.IsNullOrWhiteSpace(street))
            return Result.Failure<LocationAddress, Errors>(GeneralErrors.ValueIsNullOrWhitespace("Street"));

        if (string.IsNullOrWhiteSpace(building))
            return Result.Failure<LocationAddress, Errors>(GeneralErrors.ValueIsNullOrWhitespace("Building"));

        var fullAddress = GetFullAddress(country, city, street, building, region, district, apartment);

        if (fullAddress.Length > Constants.MAX_LOCATION_ADDRESS_LENGTH)
            errors.Add(GeneralErrors.ValueLengthIsNotValid(Constants.MAX_LOCATION_ADDRESS_LENGTH, "Address"));

        if (errors.Any())
            return Result.Failure<LocationAddress, Errors>(errors);

        var address = new LocationAddress(country, city, street, building, region, district, apartment);

        return Result.Success<LocationAddress, Errors>(address);
    }

    private static string GetFullAddress(params string?[] addressParts)
    {
        return string.Join(SEPARATOR, addressParts.Where(s => s != null)).Trim();
    }
}