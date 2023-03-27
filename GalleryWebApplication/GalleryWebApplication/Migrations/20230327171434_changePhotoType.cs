using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalleryWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class changePhotoType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Products");

            migrationBuilder.AddColumn<Guid>(
                name: "PhotoId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
