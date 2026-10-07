using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechStore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class m007_RenameProductAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValue_CategoryAttribute_CategoryAttributeId",
                table: "ProductAttributeValue");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValue_Products_ProductId",
                table: "ProductAttributeValue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductAttributeValue",
                table: "ProductAttributeValue");

            migrationBuilder.RenameTable(
                name: "ProductAttributeValue",
                newName: "ProductAttributeValues");

            migrationBuilder.RenameIndex(
                name: "IX_ProductAttributeValue_ProductId",
                table: "ProductAttributeValues",
                newName: "IX_ProductAttributeValues_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductAttributeValue_CategoryAttributeId",
                table: "ProductAttributeValues",
                newName: "IX_ProductAttributeValues_CategoryAttributeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductAttributeValues",
                table: "ProductAttributeValues",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValues_CategoryAttribute_CategoryAttributeId",
                table: "ProductAttributeValues",
                column: "CategoryAttributeId",
                principalTable: "CategoryAttribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValues_Products_ProductId",
                table: "ProductAttributeValues",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValues_CategoryAttribute_CategoryAttributeId",
                table: "ProductAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValues_Products_ProductId",
                table: "ProductAttributeValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductAttributeValues",
                table: "ProductAttributeValues");

            migrationBuilder.RenameTable(
                name: "ProductAttributeValues",
                newName: "ProductAttributeValue");

            migrationBuilder.RenameIndex(
                name: "IX_ProductAttributeValues_ProductId",
                table: "ProductAttributeValue",
                newName: "IX_ProductAttributeValue_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductAttributeValues_CategoryAttributeId",
                table: "ProductAttributeValue",
                newName: "IX_ProductAttributeValue_CategoryAttributeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductAttributeValue",
                table: "ProductAttributeValue",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValue_CategoryAttribute_CategoryAttributeId",
                table: "ProductAttributeValue",
                column: "CategoryAttributeId",
                principalTable: "CategoryAttribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValue_Products_ProductId",
                table: "ProductAttributeValue",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
