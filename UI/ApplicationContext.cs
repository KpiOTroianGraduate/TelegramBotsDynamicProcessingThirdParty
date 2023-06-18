using Microsoft.EntityFrameworkCore;
using UI.Entities;

namespace UI;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
}