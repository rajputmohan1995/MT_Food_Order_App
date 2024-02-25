using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MT.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class addusernameinorderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "OrderHeaders");
        }
    }
}
