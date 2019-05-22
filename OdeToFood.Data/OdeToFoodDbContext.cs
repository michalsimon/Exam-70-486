namespace OdeToFood.Data
{
    using Microsoft.EntityFrameworkCore;
    using OdeToFood.Core;

    public class OdeToFoodDbContext : DbContext
    {
        public OdeToFoodDbContext(DbContextOptions<OdeToFoodDbContext> options)
            : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants { get; set; }
    }
}