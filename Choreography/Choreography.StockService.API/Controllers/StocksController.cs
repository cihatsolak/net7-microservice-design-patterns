namespace Choreography.StockService.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public StocksController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> All(CancellationToken cancellationToken)
        {
            var stocks = await _dbContext.Stocks.ToListAsync(cancellationToken);
            if (!stocks.Any())
            {
                return NotFound();
            }

            return Ok(stocks);
        }
    }
}
