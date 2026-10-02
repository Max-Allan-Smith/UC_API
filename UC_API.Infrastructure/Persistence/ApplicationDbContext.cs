using Microsoft.EntityFrameworkCore;

namespace UC_API.Infrastructure.Persistence
{
    public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
    };
}