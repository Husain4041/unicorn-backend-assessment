using Microsoft.EntityFrameworkCore;
using UnicornBackend.Models;    

namespace UnicornBackend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<MakerReview> MakerReviews => Set<MakerReview>();
    public DbSet<CheckerReview> CheckerReviews => Set<CheckerReview>();
}
