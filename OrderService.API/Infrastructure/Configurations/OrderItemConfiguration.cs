namespace OrderService.API.Infrastructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable(nameof(OrderItem), "dbo");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).UseIdentityColumn(1, 1).ValueGeneratedOnAdd().IsRequired();
            builder.Property(p => p.ProductId).IsRequired();
            builder.Property(p => p.Count).IsRequired();
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
