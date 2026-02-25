using System.Data;

namespace DirectoryService.Application.Abstractions.Database;

public interface IDbConnectionFactory
{
    IDbConnection GetDbConnection();
}