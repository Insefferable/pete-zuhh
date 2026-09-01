using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace PizzaStore.Api.Infrastructure;

public static class DbUpdateExceptionExtensions
{
    private const int SqliteConstraintErrorCode = 19;
    public static bool IsUniqueConstraintViolation(this DbUpdateException ex)
    {
        return ex.InnerException is SqliteException sqliteEx 
            && sqliteEx.SqliteErrorCode == SqliteConstraintErrorCode 
            && sqliteEx.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase);
    }
}