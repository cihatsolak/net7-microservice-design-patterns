namespace EventSourcing.API.Command.CreateProduct
{
    public class CreateProductCommand : IRequest
    {
        public CreateProductDto CreateProductDto { get; set; }

        public CreateProductCommand()
        {
        }

        public CreateProductCommand(CreateProductDto createProductDto)
        {
            CreateProductDto = createProductDto;
        }
        
    }
}
