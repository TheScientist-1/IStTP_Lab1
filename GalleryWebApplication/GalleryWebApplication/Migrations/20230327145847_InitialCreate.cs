using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalleryWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.CreateTable(
            //     name: "Authors",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //         Contacts = table.Column<string>(type: "ntext", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Authors", x => x.Id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "Categories",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //         Info = table.Column<string>(type: "ntext", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Category", x => x.Id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "Customers",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //         Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //         Contacts = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Customers", x => x.Id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "Statuses",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Statuses", x => x.Id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "Products",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //         CategoryId = table.Column<int>(type: "int", nullable: false),
            //         Price = table.Column<decimal>(type: "money", nullable: false),
            //         Info = table.Column<string>(type: "ntext", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Products", x => x.Id);
            //         table.ForeignKey(
            //             name: "FK_Products_Categories",
            //             column: x => x.CategoryId,
            //             principalTable: "Categories",
            //             principalColumn: "Id");
            //     });

            // migrationBuilder.CreateTable(
            //     name: "Orders",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         CustomerId = table.Column<int>(type: "int", nullable: false),
            //         StatusId = table.Column<int>(type: "int", nullable: false),
            //         Info = table.Column<string>(type: "ntext", nullable: true),
            //         Date = table.Column<DateTime>(type: "date", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Orders", x => x.Id);
            //         table.ForeignKey(
            //             name: "FK_Orders_Customers",
            //             column: x => x.CustomerId,
            //             principalTable: "Customers",
            //             principalColumn: "Id");
            //         table.ForeignKey(
            //             name: "FK_Orders_Statuses",
            //             column: x => x.StatusId,
            //             principalTable: "Statuses",
            //             principalColumn: "Id");
            //     });

            // migrationBuilder.CreateTable(
            //     name: "AuthorsProducts",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         AuthorId = table.Column<int>(type: "int", nullable: false),
            //         ProductId = table.Column<int>(type: "int", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_AuthorsProducts", x => x.Id);
            //         table.ForeignKey(
            //             name: "FK_AuthorsProducts_Authors",
            //             column: x => x.AuthorId,
            //             principalTable: "Authors",
            //             principalColumn: "Id");
            //         table.ForeignKey(
            //             name: "FK_AuthorsProducts_Products",
            //             column: x => x.ProductId,
            //             principalTable: "Products",
            //             principalColumn: "Id");
            //     });

            // migrationBuilder.CreateTable(
            //     name: "OrderProducts",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         OrderId = table.Column<int>(type: "int", nullable: false),
            //         ProductId = table.Column<int>(type: "int", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_OrderProducts", x => x.Id);
            //         table.ForeignKey(
            //             name: "FK_OrderProducts_Orders",
            //             column: x => x.OrderId,
            //             principalTable: "Orders",
            //             principalColumn: "Id");
            //         table.ForeignKey(
            //             name: "FK_OrderProducts_Products",
            //             column: x => x.ProductId,
            //             principalTable: "Products",
            //             principalColumn: "Id");
            //     });

            // migrationBuilder.CreateIndex(
            //     name: "IX_AuthorsProducts_AuthorId",
            //     table: "AuthorsProducts",
            //     column: "AuthorId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_AuthorsProducts_ProductId",
            //     table: "AuthorsProducts",
            //     column: "ProductId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_OrderProducts_OrderId",
            //     table: "OrderProducts",
            //     column: "OrderId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_OrderProducts_ProductId",
            //     table: "OrderProducts",
            //     column: "ProductId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Orders_CustomerId",
            //     table: "Orders",
            //     column: "CustomerId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Orders_StatusId",
            //     table: "Orders",
            //     column: "StatusId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Products_CategoryId",
            //     table: "Products",
            //     column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorsProducts");

            migrationBuilder.DropTable(
                name: "OrderProducts");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
