namespace OrderService.API.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(nameof(Order), "dbo");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).UseIdentityColumn(1, 1).ValueGeneratedOnAdd().IsRequired();
            builder.Property(p => p.CreatedDate).ValueGeneratedOnAdd().HasDefaultValueSql("getdate()").IsRequired();
            builder.Property(p => p.Status).IsRequired();

            builder.OwnsOne(p => p.Address, navigationBuilder =>
            {
                navigationBuilder.Property(p => p.Line).HasMaxLength(50).IsUnicode(false);
                navigationBuilder.Property(p => p.Province).HasMaxLength(50).IsUnicode(false);
                navigationBuilder.Property(p => p.District).HasMaxLength(50).IsUnicode(false);
            });

            builder.HasMany(p => p.OrderItems).WithOne(p => p.Order).HasForeignKey(p => p.OrderId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
