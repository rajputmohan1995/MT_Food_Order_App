using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MT.Services.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class initialmigrationproductapi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryName", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Appetizer", "A samosa is a fried South Asian pastry with a savoury filling, including ingredients such as spiced potatoes, onions, peas, meat, or fish. It may take different forms, including triangular, cone, or half-moon shapes, depending on the region.", "https://static.toiimg.com/thumb/61050397.cms?imgsize=246859&width=800&height=800", "Samosa", 20.0 },
                    { 2, "Appetizer", "Paneer tikka or Paneer Soola or Chhena Soola is an Indian dish made from chunks of paneer/ chhena marinated in spices and grilled in a tandoor. It is a vegetarian alternative to chicken tikka and other meat dishes. It is a popular dish that is widely available in India and countries with an Indian diaspora.", "https://carolinescooking.com/wp-content/uploads/2021/09/paneer-tikka-featured-pic-sq.jpg", "Paneer Tikka", 200.0 },
                    { 3, "Entrée", "Pav bhaji is a street food dish from India consisting of a thick vegetable curry served with a soft bread roll. It originated in the city of Mumbai.", "https://hebbarskitchen.com/wp-content/uploads/mainPhotos/pav-bhaji-recipe-easy-mumbai-style-pav-bhaji-recipe-1-696x927.jpeg", "Pav Bhaji", 160.0 },
                    { 4, "Dessert", "Fruity, nutty or chocolatey, decked out with pastry, meringue or crumble, these sumptuous desserts are sure to satisfy your sweet tooth.", "https://media-cldnry.s-nbcnews.com/image/upload/newscms/2022_35/1808346/patti-labelle-sweet-potato-pie-te-main-211123.jpg", "Sweet Pie", 150.0 },
                    { 5, "Dessert", "Gulab jamun is a sweet confectionary or dessert, originating in the Indian subcontinent and a type of mithai popular in India, Pakistan, Nepal, the Maldives, and Bangladesh, as well as Myanmar.", "https://www.cookwithkushi.com/wp-content/uploads/2023/07/easy-juicy-gulab-jamun.jpg", "Gulab Jamun", 40.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
