namespace EventSourcing.API.Store
{
    public class ProductStream : BaseStream
    {
        internal const string StreamName = "ProductStream";
        internal const string GroupName = "agroup";
        //internal const string GroupName = "replay";

        public ProductStream(IEventStoreConnection eventStoreConnection) : base(StreamName, eventStoreConnection)
        {
        }

        public void Created(CreateProductDto createProductDto)
        {
            ProductCreatedEvent productCreatedEvent = new()
            {
                Id = Guid.NewGuid(),
                Name = createProductDto.Name,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
                UserId = createProductDto.UserId
            };

            Events.AddLast(productCreatedEvent);
        }

        public void NameChanged(ChangeProductNameDto changeProductNameDto)
        {
            ProductNameChangedEvent productNameChangedEvent = new()
            {
                ChangedName = changeProductNameDto.Name,
                Id = changeProductNameDto.Id
            };

            Events.AddLast(productNameChangedEvent);
        }

        public void PriceChanged(ChangeProductPriceDto changeProductPriceDto)
        {
            ProductPriceChangedEvent productPriceChangedEvent = new()
            {
                ChangedPrice = changeProductPriceDto.Price,
                Id = changeProductPriceDto.Id
            };

            Events.AddLast(productPriceChangedEvent);
        }

        public void Deleted(Guid id)
        {
            ProductDeletedEvent productDeletedEvent = new() { Id = id };

            Events.AddLast(productDeletedEvent);
        }
    }
}
