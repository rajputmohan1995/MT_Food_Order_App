using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MT.Services.AuthAPI.Migrations
{
    /// <inheritdoc />
    public partial class addmoreuserdetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingCity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingCountry",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingState",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingZipCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingCity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingCountry",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingState",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingZipCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BillingCity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BillingCountry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BillingState",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BillingZipCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShippingCity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShippingCountry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShippingState",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShippingZipCode",
                table: "AspNetUsers");
        }
    }
}
