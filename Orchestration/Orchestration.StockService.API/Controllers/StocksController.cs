namespace Orchestration.StockService.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StocksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> All(CancellationToken cancellationToken)
        {
            var stocks = await _context.Stocks.ToListAsync(cancellationToken);
            if (!stocks.Any())
            {
                return NotFound();
            }

            return Ok(stocks);
        }
    }
}
