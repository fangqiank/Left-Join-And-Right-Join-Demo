using LeftJoinAndRightJoinDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace LeftJoinAndRightJoinDemo.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "High performance laptop", Price = 999.99m, CreatedDate = DateTime.UtcNow.AddDays(-30) },
                new Product { Id = 2, Name = "Smartphone", Description = "Latest smartphone model", Price = 699.99m, CreatedDate = DateTime.UtcNow.AddDays(-20) },
                new Product { Id = 3, Name = "Headphones", Description = "Noise cancelling headphones", Price = 199.99m, CreatedDate = DateTime.UtcNow.AddDays(-10) },
                new Product { Id = 4, Name = "Smart Watch", Description = "Fitness tracking watch", Price = 299.99m, CreatedDate = DateTime.UtcNow.AddDays(-5) },
                new Product { Id = 5, Name = "Tablet", Description = "10-inch tablet", Price = 399.99m, CreatedDate = DateTime.UtcNow.AddDays(-15), IsActive = false }
            );

            modelBuilder.Entity<Review>().HasData(
                new Review { Id = 1, Title = "Great laptop!", Content = "Very fast and reliable", Rating = 5, ReviewerName = "John Doe", ProductId = 1, ReviewDate = DateTime.UtcNow.AddDays(-25) },
                new Review { Id = 2, Title = "Battery life could be better", Content = "Performance is good but battery drains quickly", Rating = 3, ReviewerName = "Jane Smith", ProductId = 1, ReviewDate = DateTime.UtcNow.AddDays(-20) },
                new Review { Id = 3, Title = "Excellent phone", Content = "Camera quality is amazing", Rating = 5, ReviewerName = "Mike Johnson", ProductId = 2, ReviewDate = DateTime.UtcNow.AddDays(-15) },
                new Review { Id = 4, Title = "Best headphones ever", Content = "Noise cancellation works perfectly", Rating = 5, ReviewerName = "Sarah Williams", ProductId = 3, ReviewDate = DateTime.UtcNow.AddDays(-5) },
                new Review { Id = 5, Title = "Good but pricey", Content = "Features are good but too expensive", Rating = 4, ReviewerName = "Tom Brown", ProductId = 3, ReviewDate = DateTime.UtcNow.AddDays(-3) },
                new Review { Id = 6, Title = "Disappointing", Content = "Broke after 2 weeks", Rating = 1, ReviewerName = "Alex Green", ProductId = 3, ReviewDate = DateTime.UtcNow.AddDays(-1) },
                new Review { Id = 7, Title = "Average watch", Content = "Does the job but nothing special", Rating = 3, ReviewerName = "Lisa White", ProductId = 4, ReviewDate = DateTime.UtcNow.AddDays(-2) },
                // Product 5 (Tablet) has no reviews - for demonstrating LEFT JOIN
                // Review 8-10: Invalid ProductId - for demonstrating RIGHT JOIN
                new Review { Id = 8, Title = "Ghost review 1", Content = "This review has invalid product", Rating = 2, ReviewerName = "Ghost Reviewer", ProductId = 999, ReviewDate = DateTime.UtcNow.AddDays(-1) },
                new Review { Id = 9, Title = "Ghost review 2", Content = "Another invalid product review", Rating = 4, ReviewerName = "Unknown", ProductId = 888, ReviewDate = DateTime.UtcNow.AddDays(-2) }
            );
        }
    }
}
