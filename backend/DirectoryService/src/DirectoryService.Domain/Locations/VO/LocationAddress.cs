using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace DirectoryService.Domain.Locations.VO;

public record LocationAddress
{
    // ef core
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

    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string Building { get; init; } = string.Empty;
    public string? Region { get; init; }
    public string? District { get; init; }
    public string? Apartment { get; init; }
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
            errors.Add(GeneralErrors.ValueLengthIsNotValid(Constants.MAX_LOCATION_ADDRESS_LENGTH));

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