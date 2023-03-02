namespace Choreography.StockService.API.Infrastructure.Context
{
    public static class AppDbContextSeed
    {
        public static async Task ExecuteAsync(this WebApplication webApplication)
        {
            using var scope = webApplication.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<Stock> stocks = new()
            {
                new Stock
                {
                    Id = 1,
                    ProductId = 1,
                    Count = 100
                },
                new Stock
                {
                    Id = 2,
                    ProductId = 2,
                    Count = 100
                }
            };

            await context.Stocks.AddRangeAsync(stocks);
            await context.SaveChangesAsync();
        }
    }
}
