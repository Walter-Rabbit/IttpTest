using IttpTest.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace IttpTest.Data;

public class IttpContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public IttpContext(DbContextOptions options) : base(options)
    {
    }
}