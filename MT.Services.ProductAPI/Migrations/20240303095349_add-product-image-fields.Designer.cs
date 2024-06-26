﻿// <auto-generated />
using MT.Services.ProductAPI.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MT.Services.ProductAPI.Migrations
{
    [DbContext(typeof(ProductDbContext))]
    [Migration("20240303095349_add-product-image-fields")]
    partial class addproductimagefields
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MT.Services.ProductAPI.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageLocalPathUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("ProductId");

                    b.ToTable("Products", t =>
                        {
                            t.HasCheckConstraint("Check_Product_Price", "[Price] > 0 AND [Price] <= 1000");
                        });

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            CategoryName = "Appetizer",
                            Description = "A samosa is a fried South Asian pastry with a savoury filling, including ingredients such as spiced potatoes, onions, peas, meat, or fish. It may take different forms, including triangular, cone, or half-moon shapes, depending on the region.",
                            ImageUrl = "https://static.toiimg.com/thumb/61050397.cms?imgsize=246859&width=800&height=800",
                            Name = "Samosa",
                            Price = 20.0
                        },
                        new
                        {
                            ProductId = 2,
                            CategoryName = "Appetizer",
                            Description = "Paneer tikka or Paneer Soola or Chhena Soola is an Indian dish made from chunks of paneer/ chhena marinated in spices and grilled in a tandoor. It is a vegetarian alternative to chicken tikka and other meat dishes. It is a popular dish that is widely available in India and countries with an Indian diaspora.",
                            ImageUrl = "https://carolinescooking.com/wp-content/uploads/2021/09/paneer-tikka-featured-pic-sq.jpg",
                            Name = "Paneer Tikka",
                            Price = 200.0
                        },
                        new
                        {
                            ProductId = 3,
                            CategoryName = "Entrée",
                            Description = "Pav bhaji is a street food dish from India consisting of a thick vegetable curry served with a soft bread roll. It originated in the city of Mumbai.",
                            ImageUrl = "https://hebbarskitchen.com/wp-content/uploads/mainPhotos/pav-bhaji-recipe-easy-mumbai-style-pav-bhaji-recipe-1-696x927.jpeg",
                            Name = "Pav Bhaji",
                            Price = 160.0
                        },
                        new
                        {
                            ProductId = 4,
                            CategoryName = "Dessert",
                            Description = "Fruity, nutty or chocolatey, decked out with pastry, meringue or crumble, these sumptuous desserts are sure to satisfy your sweet tooth.",
                            ImageUrl = "https://media-cldnry.s-nbcnews.com/image/upload/newscms/2022_35/1808346/patti-labelle-sweet-potato-pie-te-main-211123.jpg",
                            Name = "Sweet Pie",
                            Price = 150.0
                        },
                        new
                        {
                            ProductId = 5,
                            CategoryName = "Dessert",
                            Description = "Gulab jamun is a sweet confectionary or dessert, originating in the Indian subcontinent and a type of mithai popular in India, Pakistan, Nepal, the Maldives, and Bangladesh, as well as Myanmar.",
                            ImageUrl = "https://www.cookwithkushi.com/wp-content/uploads/2023/07/easy-juicy-gulab-jamun.jpg",
                            Name = "Gulab Jamun",
                            Price = 40.0
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
