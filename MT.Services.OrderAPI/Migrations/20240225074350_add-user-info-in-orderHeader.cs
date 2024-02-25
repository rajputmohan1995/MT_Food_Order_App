using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MT.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class adduserinfoinorderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "OrderHeaders",
                newName: "UserPhone");

            migrationBuilder.AddColumn<string>(
                name: "UserFullName",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserFullName",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "UserPhone",
                table: "OrderHeaders",
                newName: "CustomerName");
        }
    }
}
