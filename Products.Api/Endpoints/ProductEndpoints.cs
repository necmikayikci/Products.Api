using Microsoft.EntityFrameworkCore;
using Products.Api.Contracts;
using Products.Api.Database;
using Products.Api.Entities;

namespace Products.Api.Endpoints
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            //Create Product
            app.MapPost("products", async (CreateProductRequest request, ApplicationDbContext context, CancellationToken cToken) =>
            {
                var product = new Product();
                product.Name = request.Name;
                product.Price = request.Price;

                context.Add(product);

                await context.SaveChangesAsync(cToken);

                return Results.Ok(product);
            });

            //Update Product
            app.MapPut("products/{id}", async (int id, UpdateProductRequest request, ApplicationDbContext context, CancellationToken cToken) =>
            {
                var product = await context.Products.FirstOrDefaultAsync(p => p.Id == id, cToken);

                if (product is null)
                {
                    return Results.NotFound();
                }

                product.Name = request.Name;
                product.Price = request.Price;

                await context.SaveChangesAsync(cToken);

                return Results.NoContent();
            });

            //Get Product
            app.MapGet("products/{id}", async (int id, ApplicationDbContext context, CancellationToken cToken) =>
            {
                var product = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cToken);

                return product is null ? Results.NotFound() : Results.Ok(product);
            });

            //Get Products with paging
            app.MapGet("products", async (ApplicationDbContext context, CancellationToken ct, int page = 1, int pageSize = 10) =>
            {
                var products = await context.Products
                    .AsNoTracking()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(ct);

                return Results.Ok(products);
            });

            //Delete Product
            app.MapDelete("products/{id}", async (int id, ApplicationDbContext context, CancellationToken ct) =>
            {
                var product = await context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

                if (product is null)
                {
                    return Results.NotFound();
                }

                context.Remove(product);

                await context.SaveChangesAsync(ct);

                return Results.NoContent();
            });
        }
    }
}
