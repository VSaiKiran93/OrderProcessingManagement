using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.API.Data;

public class DbContextFixture : IDisposable
{
    public DbContextOptions<ApplicationDbContext> Options { get; }
    private ApplicationDbContext _context; // Use _context instead of _dbcontext

    public DbContextFixture()
    {
        // Initialize in-memory SQLite database
        Options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")  // In-memory database for testing
            .Options;

        // Ensure the database is created (if it's not already)
        _context = new ApplicationDbContext(Options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        // Dispose the context to close connections and clean up resources
        _context?.Dispose(); // Make sure to refer to _context
    }
}
