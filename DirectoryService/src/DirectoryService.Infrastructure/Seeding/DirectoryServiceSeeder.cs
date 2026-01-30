using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.VO;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.VO;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Seeding;

public class DirectoryServiceSeeder : ISeeder
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<DirectoryServiceSeeder> _logger;

    private readonly Random _random = new();

    private const int DEPARTMENTS_COUNT = 100;
    private readonly int LOCATIONS_COUNT = 100;
    private readonly int POSITIONS_COUNT = 100;

    public DirectoryServiceSeeder(
        DirectoryServiceDbContext dbContext,
        ILogger<DirectoryServiceSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting seeding directory service data");

        try
        {
            await SeedData();

            _logger.LogInformation("Finished seeding directory service data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errors occurred while seeding directory service data");
        }
    }

    private async Task SeedData()
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await ClearDatabase();
            var locations = await SeedLocations();
            var departments = await SeedDepartments(locations);
            await SeedPositions(departments);

            await transaction.CommitAsync();
            _logger.LogInformation("Seeding completed successfully");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw; 
        }
    }

    private async Task ClearDatabase()
    {
        _logger.LogInformation("Clearing database tables");

        await _dbContext.Database.ExecuteSqlRawAsync(@"
        TRUNCATE TABLE department_position, department_location, positions, locations, departments 
        RESTART IDENTITY CASCADE");
    }

    private async Task<List<Location>> SeedLocations()
    {
        _logger.LogInformation("Seeding {Count} locations", LOCATIONS_COUNT);

        var locations = new List<Location>();
        var cities = new[] { "Moscow", "SPb", "Novosibirsk", "Ekaterinburg", "Kazan", "Omsk", "Rostov", "Ufa" };
        var streets = new[] { "Lenina", "Pushkina", "Gogolya", "Kirova", "Marx" };
        var timezones = new[] { "Europe/Moscow", "Asia/Yekaterinburg", "Asia/Omsk", "Asia/Novosibirsk" };

        for (int i = 0; i < LOCATIONS_COUNT; i++)
        {
            var cityIdx = i % cities.Length;
            var streetIdx = i % streets.Length;  

            var address = LocationAddress.Create(
                country: "Russia",
                city: $"{cities[cityIdx]}-{i:D2}",           
                street: $"{streets[streetIdx]} St",         
                building: $"{i + 1:D3}",                   
                region: $"RU-{i % 4 + 1}"                    
            ).Value;

            var name = LocationName.Create($"Office-{i:D3}-{cities[cityIdx]}").Value;
            var timezone = LocationTimezone.Create(timezones[i % timezones.Length]).Value;

            var location = Location.Create(Guid.NewGuid(), name, address, timezone).Value;
            _dbContext.Locations.Add(location);
            locations.Add(location);
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Created {Count} UNIQUE locations", locations.Count);
        return locations;
    }

    private async Task<List<Department>> SeedDepartments(List<Location> locations)
    {
        _logger.LogInformation("Seeding {Count} departments", DEPARTMENTS_COUNT);
        var departments = new List<Department>();
        var rootDepartments = new List<Department>();

        var latinLetters = "abcdefghijkmnopqrstuvwxyz";

        for (int i = 0; i < 10; i++)
        {
            var departmentId = Guid.NewGuid();

            var identifierValue = $"{latinLetters[i]}div";  
            var nameValue = $"Division {i + 1}";

            var identifier = DepartmentIdentifier.Create(identifierValue).Value;
            var name = DepartmentName.Create(nameValue).Value;

            var department = Department.CreateParent(
                departmentId, 
                name, 
                identifier, 
                [DepartmentLocation.Create(departmentId, locations[i].Id).Value]
            ).Value;

            _dbContext.Departments.Add(department);
            rootDepartments.Add(department);
            departments.Add(department);
        }
        await _dbContext.SaveChangesAsync();

        var childPrefixes = new[] { "dep", "unit", "team", "sect", "grp" };
        for (int i = 0; i < DEPARTMENTS_COUNT - 10; i++)
        {
            var departmentId = Guid.NewGuid();

            var parent = rootDepartments[_random.Next(rootDepartments.Count)];
            var prefix = childPrefixes[i % childPrefixes.Length];
            var letter1 = latinLetters[i % latinLetters.Length];
            var letter2 = latinLetters[(i / 10) % latinLetters.Length];

            var identifierValue = $"{prefix}{letter1}{letter2}";  
            var nameValue = $"Department {i + 1}";

            var identifier = DepartmentIdentifier.Create(identifierValue).Value;
            var name = DepartmentName.Create(nameValue).Value;

            var department = Department.CreateChild(
                departmentId,
                name, 
                identifier,
                parent,
                [DepartmentLocation.Create(departmentId, locations[i + 10].Id).Value]
            ).Value;

            _dbContext.Departments.Add(department);
            departments.Add(department);
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Created {Count} departments", departments.Count);
        return departments;
    }

    private async Task SeedPositions(List<Department> departments)
    {
        _logger.LogInformation("Seeding {Count} positions", POSITIONS_COUNT);

        var positionPrefixes = new[] { "Mgr", "SrDev", "Dev", "QA", "DevOps", "Designer", "Analyst" };
        var descriptions = new[]
        {
        "Manages department operations and team leads",
        "Senior software engineer with 5+ years experience",
        "Software developer implementing business features",
        "Quality assurance engineer testing applications",
        "Infrastructure and deployment automation specialist",
        "UI/UX designer creating user interfaces",
        "Business analyst gathering requirements and documentation"
        };

        for (int i = 0; i < POSITIONS_COUNT; i++)
        {
            var positionId = Guid.NewGuid();

            var prefix = positionPrefixes[i % positionPrefixes.Length];
            var uniqueName = $"{prefix}-{i:D3}";
            var description = descriptions[i % descriptions.Length];

            var positionName = PositionName.Create(uniqueName).Value;
            var position = Position.Create(
                id: positionId,
                name: positionName,
                description: description,
                departments: [DepartmentPosition.Create(departments[i].Id,  positionId).Value]
            ).Value;

            _dbContext.Positions.Add(position);
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Created {Count} UNIQUE positions", POSITIONS_COUNT);
    }
}

