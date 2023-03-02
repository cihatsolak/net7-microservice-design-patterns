using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderFailMessageProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailMessage",
                schema: "dbo",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailMessage",
                schema: "dbo",
                table: "Order");
        }
    }
}
