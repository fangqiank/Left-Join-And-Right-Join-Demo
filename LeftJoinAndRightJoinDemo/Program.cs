using LeftJoinAndRightJoinDemo.Data;
using LeftJoinAndRightJoinDemo.DTOs;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ProductReviewDb")
);

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// 1. LEFT JOIN example: Get all products with their reviews (including products without reviews)
app.MapGet("/products/with-reviews", (AppDbContext dbContext) =>
{
    var query = dbContext.Products
        .Where(p => p.IsActive)
        //.AsEnumerable()
        .LeftJoin(
            dbContext.Reviews,
            product => product.Id,
            review => review.ProductId,
            (product, review) => new ProductReviewDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductPrice = product.Price,
                ReviewId = review != null ? review.Id : null,
                ReviewTitle = review != null ? review.Title : null,
                ReviewRating = review != null ? review.Rating : null,
                ReviewerName = review != null ? review.ReviewerName : null
            })
        .OrderBy(r => r.ProductId)
        .ThenBy(r => r.ReviewId ?? 0);

    var results = query.ToList();
    return Results.Ok(results);
});

// 2. Using custom RIGHT JOIN: Get all reviews including those for non-existent products(不加AsEnumerable(),会报错)
//app.MapGet("/reviews/right-join", (AppDbContext dbContext) =>
//{
//    var query = dbContext.Products
//        .AsEnumerable()
//        .RightJoin(
//            dbContext.Reviews,
//            product => product.Id,
//            review => review.ProductId,
//            (product, review) => new ProductReviewDto
//            {
//                ProductId = product != null ? product.Id : 0,
//                ProductName = product != null ? product.Name : "Product Not Found",
//                ProductPrice = product != null ? product.Price : 0,
//                ReviewId = review.Id,
//                ReviewTitle = review.Title,
//                ReviewRating = review.Rating,
//                ReviewerName = review.ReviewerName
//            })
//        .OrderBy(r => r.ReviewId);

//    var results = query.ToList();
//    return Results.Ok(results);
//});
app.MapGet("/reviews/right-join", (AppDbContext dbContext) =>
{
    var query = from review in dbContext.Reviews
                join product in dbContext.Products
                on review.ProductId equals product.Id into productGroup
                from product in productGroup.DefaultIfEmpty()
                orderby review.Id
                select new ProductReviewDto
                {
                    ProductId = product != null ? product.Id : 0,
                    ProductName = product != null ? product.Name : "Product Not Found",
                    ProductPrice = product != null ? product.Price : 0,
                    ReviewId = review.Id,
                    ReviewTitle = review.Title,
                    ReviewRating = review.Rating,
                    ReviewerName = review.ReviewerName
                };

    var results = query.ToList();
    return Results.Ok(results);
});

// 3. Using INNER JOIN: Get only products that have reviews
app.MapGet("/products/inner-join", async (AppDbContext dbContext) =>
{
    var query = dbContext.Products
        .Where(p => p.IsActive)
        .Join(
            dbContext.Reviews,
            product => product.Id,
            review => review.ProductId,
            (product, review) => new ProductReviewDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductPrice = product.Price,
                ReviewId = review.Id,
                ReviewTitle = review.Title,
                ReviewRating = review.Rating,
                ReviewerName = review.ReviewerName
            })
        .OrderBy(r => r.ProductId)
        .ThenBy(r => r.ReviewId);

    var result = await query.ToListAsync();
    return Results.Ok(result);
});

// 4. Aggregated LEFT JOIN example: Get products with review count statistics
app.MapGet("/products/left-join-summary", async (AppDbContext dbContext) =>
{
    var result = await dbContext.Products
        .Where(p => p.IsActive)
        .Select(product => new
        {
            product.Id,
            product.Name,
            product.Price,
            Reviews = dbContext.Reviews
                .Where(r => r.ProductId == product.Id)
                .ToList()
        })
        .Select(x => new ProductReviewSummaryDto
        {
            ProductId = x.Id,
            ProductName = x.Name,
            ReviewCount = x.Reviews.Count,
            AverageRating = x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0
        })
        .OrderBy(x => x.ProductName)
        .ToListAsync();

    return Results.Ok(result);
});

// 5. Using GroupJoin for LEFT JOIN operations
app.MapGet("/products/groupjoin-left", async (AppDbContext dbContext) =>
{
    var query = dbContext.Products
        .Where(p => p.IsActive)
        .GroupJoin(
            dbContext.Reviews,
            product => product.Id,
            review => review.ProductId,
            (product, reviews) => new { Product = product, Reviews = reviews })
        .SelectMany(
            x => x.Reviews.DefaultIfEmpty(),
            (x, review) => new ProductReviewDto
            {
                ProductId = x.Product.Id,
                ProductName = x.Product.Name,
                ProductPrice = x.Product.Price,
                ReviewId = review != null ? review.Id : null,
                ReviewTitle = review != null ? review.Title : null,
                ReviewRating = review != null ? review.Rating : null,
                ReviewerName = review != null ? review.ReviewerName : null
            })
        .OrderBy(r => r.ProductId)
        .ThenBy(r => r.ReviewId ?? 0);

    var result = await query.ToListAsync();
    return Results.Ok(result);
});

// 6. Filtered LEFT JOIN: Get products with 4+ star ratings only
app.MapGet("/products/left-join-filtered", (AppDbContext dbContext) =>
{
    var query = dbContext.Products
        .Where(p => p.IsActive && p.Price > 100)
        .LeftJoin(
            dbContext.Reviews.Where(r => r.Rating >= 4),
            product => product.Id,
            review => review.ProductId,
            (product, review) => new ProductReviewDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductPrice = product.Price,
                ReviewId = review != null ? review.Id : null,
                ReviewTitle = review != null ? review.Title : null,
                ReviewRating = review != null ? review.Rating : null,
                ReviewerName = review != null ? review.ReviewerName : null
            })
        .Where(x => x.ReviewRating != null) // Only products with high ratings
        .OrderByDescending(r => r.ReviewRating)
        .ThenBy(r => r.ProductName);

    var result = query.ToList();
    return Results.Ok(result);
});

// 7. Multi-condition LEFT JOIN example
app.MapGet("/products/left-join-multi", (AppDbContext dbContext) =>
{
    // Get reviews from the last 7 days
    var recentDate = DateTime.UtcNow.AddDays(-7);

    var query = dbContext.Products
        .Where(p => p.IsActive)
        .LeftJoin(
            dbContext.Reviews.Where(r => r.ReviewDate >= recentDate),
            product => product.Id,
            review => review.ProductId,
            (product, review) => new
            {
                Product = product,
                Review = review
            })
        .OrderBy(x => x.Product.Id)
        .Select(x => new
        {
            ProductId = x.Product.Id,
            ProductName = x.Product.Name,
            HasRecentReview = x.Review != null,
            RecentReviewTitle = x.Review != null ? x.Review.Title : "No recent reviews",
            RecentReviewer = x.Review != null ? x.Review.ReviewerName : "N/A"
        });

    var result = query.ToList();
    return Results.Ok(result);
});

// 8. Complex query using LEFT JOIN: Get each product's best review
app.MapGet("/products/best-reviews", (AppDbContext dbContext) =>
{
    // Get the highest rating for each product
    var bestRatings = dbContext.Reviews
        .GroupBy(r => r.ProductId)
        .Select(g => new
        {
            ProductId = g.Key,
            MaxRating = g.Max(r => r.Rating)
        });

    var query = dbContext.Products
        .Where(p => p.IsActive)
        .LeftJoin(
            bestRatings,
            product => product.Id,
            rating => rating.ProductId,
            (product, rating) => new { Product = product, MaxRating = rating != null ? rating.MaxRating : 0 })
        .LeftJoin(
            dbContext.Reviews,
            x => new { ProductId = x.Product.Id, Rating = x.MaxRating },
            review => new { review.ProductId, Rating = review.Rating },
            (x, review) => new
            {
                ProductId = x.Product.Id,
                ProductName = x.Product.Name,
                ProductPrice = x.Product.Price,
                BestRating = x.MaxRating,
                BestReviewTitle = review != null ? review.Title : "No reviews",
                ReviewerName = review != null ? review.ReviewerName : "N/A"
            })
        .OrderByDescending(x => x.BestRating)
        .ThenBy(x => x.ProductName);

    var result = query.ToList();
    return Results.Ok(result);
});

app.MapGet("/products", async (AppDbContext dbContext) =>
{
    var products = await dbContext.Products
        .Where(p => p.IsActive)
        .ToListAsync();
    return Results.Ok(products);
});

app.MapGet("/reviews", async (AppDbContext dbContext) =>
{
    var reviews = await dbContext.Reviews
        .ToListAsync();
    return Results.Ok(reviews);
});

app.MapGet("/products/{id}/reviews", async (int id, AppDbContext dbContext) =>
{
    var reviews = await dbContext.Reviews
        .Where(r => r.ProductId == id)
        .ToListAsync();

    if (!reviews.Any())
        return Results.NotFound($"No reviews found for product {id}");

    return Results.Ok(reviews);
});

app.Run();

