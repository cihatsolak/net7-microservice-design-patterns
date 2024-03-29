﻿namespace EventSourcing.API.Store
{
    public class ProductStream : BaseStream
    {
        internal const string STREAM_NAME = "ProductStream";
        internal const string A_GROUP_NAME = "agroup";
        internal const string REPLAY_GROUP_NAME = "replay";

        public ProductStream(EventStoreClient eventStoreClient) : base(STREAM_NAME, eventStoreClient)
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
