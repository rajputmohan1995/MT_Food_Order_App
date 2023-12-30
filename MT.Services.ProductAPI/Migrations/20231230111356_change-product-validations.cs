using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MT.Services.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class changeproductvalidations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "Check_Product_Price",
                table: "Products",
                sql: "[Price] > 0 AND [Price] <= 1000");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "Check_Product_Price",
                table: "Products");
        }
    }
}
