using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcomApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "productsCategories",
                columns: table => new
                {
                    Category_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image_Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productsCategories", x => x.Category_Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    User_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    first_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Last_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Strip_Cust_Id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.User_Id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Product_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image_Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Category_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Product_Id);
                    table.ForeignKey(
                        name: "FK_products_productsCategories_Category_Id",
                        column: x => x.Category_Id,
                        principalTable: "productsCategories",
                        principalColumn: "Category_Id");
                });

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    Adress_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Postal_Code = table.Column<int>(type: "int", nullable: false),
                    isPrimary = table.Column<bool>(type: "bit", nullable: false),
                    User_Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addresses", x => x.Adress_Id);
                    table.ForeignKey(
                        name: "FK_addresses_users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "users",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Order_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    price = table.Column<double>(type: "float", nullable: false),
                    Order_Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Payment_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Strip_session_Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Order_Id);
                    table.ForeignKey(
                        name: "FK_orders_users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "users",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "usersProducts",
                columns: table => new
                {
                    User_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Product_ID = table.Column<int>(type: "int", nullable: false),
                    Quntity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CartCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    cartUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usersProducts", x => new { x.User_ID, x.Product_ID });
                    table.ForeignKey(
                        name: "FK_usersProducts_products_Product_ID",
                        column: x => x.Product_ID,
                        principalTable: "products",
                        principalColumn: "Product_Id");
                    table.ForeignKey(
                        name: "FK_usersProducts_users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "users",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "ordersProducts",
                columns: table => new
                {
                    Order_Id = table.Column<int>(type: "int", nullable: false),
                    Product_Id = table.Column<int>(type: "int", nullable: false),
                    Order_Qty = table.Column<int>(type: "int", nullable: false),
                    Unite_Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordersProducts", x => new { x.Order_Id, x.Product_Id });
                    table.ForeignKey(
                        name: "FK_ordersProducts_orders_Order_Id",
                        column: x => x.Order_Id,
                        principalTable: "orders",
                        principalColumn: "Order_Id");
                    table.ForeignKey(
                        name: "FK_ordersProducts_products_Product_Id",
                        column: x => x.Product_Id,
                        principalTable: "products",
                        principalColumn: "Product_Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_addresses_User_Id",
                table: "addresses",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_User_Id",
                table: "orders",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ordersProducts_Product_Id",
                table: "ordersProducts",
                column: "Product_Id");

            migrationBuilder.CreateIndex(
                name: "IX_products_Category_Id",
                table: "products",
                column: "Category_Id");

            migrationBuilder.CreateIndex(
                name: "IX_usersProducts_Product_ID",
                table: "usersProducts",
                column: "Product_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.DropTable(
                name: "ordersProducts");

            migrationBuilder.DropTable(
                name: "usersProducts");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "productsCategories");
        }
    }
}
