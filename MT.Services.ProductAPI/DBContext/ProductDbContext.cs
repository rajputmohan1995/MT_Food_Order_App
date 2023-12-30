using Microsoft.EntityFrameworkCore;
using MT.Services.ProductAPI.Models;

namespace MT.Services.ProductAPI.DBContext;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> context) : base(context)
    { }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasCheckConstraint("Check_Product_Price", "[Price] > 0 AND [Price] <= 1000");

        modelBuilder.Entity<Product>().HasData(new Product()
        {
            ProductId = 1,
            Name = "Samosa",
            Price = 20.00,
            Description = "A samosa is a fried South Asian pastry with a savoury filling, including ingredients such as spiced potatoes, onions, peas, meat, or fish. It may take different forms, including triangular, cone, or half-moon shapes, depending on the region.",
            CategoryName = "Appetizer",
            ImageUrl = "https://static.toiimg.com/thumb/61050397.cms?imgsize=246859&width=800&height=800"
        });

        modelBuilder.Entity<Product>().HasData(new Product()
        {
            ProductId = 2,
            Name = "Paneer Tikka",
            Price = 200.00,
            Description = "Paneer tikka or Paneer Soola or Chhena Soola is an Indian dish made from chunks of paneer/ chhena marinated in spices and grilled in a tandoor. It is a vegetarian alternative to chicken tikka and other meat dishes. It is a popular dish that is widely available in India and countries with an Indian diaspora.",
            CategoryName = "Appetizer",
            ImageUrl = "https://carolinescooking.com/wp-content/uploads/2021/09/paneer-tikka-featured-pic-sq.jpg"
        });

        modelBuilder.Entity<Product>().HasData(new Product()
        {
            ProductId = 3,
            Name = "Pav Bhaji",
            Price = 160.00,
            Description = "Pav bhaji is a street food dish from India consisting of a thick vegetable curry served with a soft bread roll. It originated in the city of Mumbai.",
            CategoryName = "Entrée",
            ImageUrl = "https://hebbarskitchen.com/wp-content/uploads/mainPhotos/pav-bhaji-recipe-easy-mumbai-style-pav-bhaji-recipe-1-696x927.jpeg"
        });

        modelBuilder.Entity<Product>().HasData(new Product()
        {
            ProductId = 4,
            Name = "Sweet Pie",
            Price = 150.00,
            Description = "Fruity, nutty or chocolatey, decked out with pastry, meringue or crumble, these sumptuous desserts are sure to satisfy your sweet tooth.",
            CategoryName = "Dessert",
            ImageUrl = "https://media-cldnry.s-nbcnews.com/image/upload/newscms/2022_35/1808346/patti-labelle-sweet-potato-pie-te-main-211123.jpg"
        });

        modelBuilder.Entity<Product>().HasData(new Product()
        {
            ProductId = 5,
            Name = "Gulab Jamun",
            Price = 40.00,
            Description = "Gulab jamun is a sweet confectionary or dessert, originating in the Indian subcontinent and a type of mithai popular in India, Pakistan, Nepal, the Maldives, and Bangladesh, as well as Myanmar.",
            CategoryName = "Dessert",
            ImageUrl = "https://www.cookwithkushi.com/wp-content/uploads/2023/07/easy-juicy-gulab-jamun.jpg"
        });
    }
}