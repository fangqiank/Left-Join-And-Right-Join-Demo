# LeftJoinAndRightJoinDemo

## Project Overview
This project is a **.NET 10.0 ASP.NET Core Web API** designed to demonstrate various **Entity Framework Core JOIN operations**. It serves as a tutorial/reference implementation for understanding how to perform Left Joins, Right Joins, Inner Joins, and GroupJoins within a .NET environment using EF Core.

The project uses **.NET Aspire** for orchestration, consisting of an AppHost project (`LeftJoinAndRightJoinDemo.AppHost`) and the main service project (`LeftJoinAndRightJoinDemo`). It utilizes an **In-Memory Database** (`ProductReviewDb`) for easy setup and demonstration purposes.

## Key Technologies
- **Framework:** .NET 10.0 (ASP.NET Core)
- **Orchestration:** .NET Aspire
- **ORM:** Entity Framework Core
- **Database:** InMemory
- **API Documentation:** Scalar / OpenAPI

## Architecture & Structure
The solution follows a standard .NET Aspire structure:
- **`LeftJoinAndRightJoinDemo.AppHost/`**: The entry point for the distributed application. It orchestrates the services.
- **`LeftJoinAndRightJoinDemo/`**: The main Web API project containing the logic.
    - **`Program.cs`**: Contains all the API endpoint definitions and the core LINQ queries demonstrating different join types.
    - **`Data/AppDbContext.cs`**: Defines the EF Core DbContext and seeds sample data (Products and Reviews), including specific edge cases to visualize join differences.
    - **`Models/`**: Contains `Product` and `Review` entity definitions.
    - **`DTOs/`**: Data Transfer Objects for shaping API responses.
- **`LeftJoinAndRightJoinDemo.ServiceDefaults/`**: Shared service configuration (standard Aspire pattern).

## Building and Running

### Prerequisites
- .NET 10.0 SDK

### Running the Application
To run the full distributed application (recommended):
```bash
dotnet run --project LeftJoinAndRightJoinDemo.AppHost
```

To run just the API service:
```bash
dotnet run --project LeftJoinAndRightJoinDemo
```

## API Endpoints (Join Demonstrations)
The API exposes several endpoints specifically illustrating different LINQ/EF Core join techniques. 
**Note:** Due to current limitations in the EF Core 10 InMemory provider, `LeftJoin` and `RightJoin` operations are performed using **Client-Side Evaluation** (via `.AsEnumerable()`) to leverage the native .NET 10 LINQ operators.

- **`GET /products/with-reviews`**: Demonstrates **LEFT JOIN**. Returns all active products and their reviews (if any).
- **`GET /reviews/right-join`**: Demonstrates **RIGHT JOIN**. Returns all reviews, including those for non-existent products.
- **`GET /products/inner-join`**: Demonstrates **INNER JOIN**. Returns only products that have reviews.
- **`GET /products/left-join-summary`**: Demonstrates **LEFT JOIN with Aggregation**. Returns product summaries with review counts and average ratings.
- **`GET /products/groupjoin-left`**: Demonstrates **GroupJoin**. An alternative way to perform left joins.
- **`GET /products/left-join-filtered`**: Demonstrates **Filtered LEFT JOIN**. Returns specific products joined with specific reviews (e.g., rating >= 4).
- **`GET /products/left-join-multi`**: Demonstrates **Multi-condition LEFT JOIN**. Join based on recent dates.
- **`GET /products/best-reviews`**: Demonstrates **Complex LEFT JOIN**. Joins products with their best individual review.

## Development Conventions
- **Data Seeding**: The `OnModelCreating` method in `AppDbContext.cs` seeds data specifically designed to test join behaviors (e.g., a product with no reviews, a review with an invalid product ID).
- **Minimal API**: The project uses the Minimal API style in `Program.cs` for simplicity and readability of the demo logic.
- **Conventions**: Standard C# and ASP.NET Core naming conventions are followed.
