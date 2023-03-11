namespace Resiliency.ServiceY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            return Ok(new
            {
                Id = id,
                Name = "Pencil",
                Price = 100,
                Stock = 200,
                Category = "Pencils"
            });
        }
    }
}
