using CSharpFunctionalExtensions;
using SharedKernel;
using System.Text.Json.Serialization;

namespace DirectoryService.Domain.Locations.VO;

public record LocationAddress
{
    public const string SEPARATOR = ", ";

    private LocationAddress(
        string country,
        string city,
        string street,
        string building,
        string? region,
        string? district,
        string? apartment,
        string fullAddress)
    {
        Country = country;
        City = city;
        Street = street;
        Building = building;
        Region = region;
        District = district;
        Apartment = apartment;
        FullAddress = fullAddress;
    }

    public string Country { get; private set; }
    public string City { get; private set; }
    public string Street { get; private set; }
    public string Building { get; private set; }
    public string? Region { get; private set; }
    public string? District { get; private set; }
    public string? Apartment { get; private set; }
    [JsonIgnore]
    public string FullAddress { get; private set; }

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

        var address = new LocationAddress(country, city, street, building, region, district, apartment, fullAddress);

        return Result.Success<LocationAddress, Errors>(address);
    }

    private static string GetFullAddress(params string?[] addressParts)
    {
        return string.Join(SEPARATOR, addressParts.Where(s => s != null)).Trim();
    }
}