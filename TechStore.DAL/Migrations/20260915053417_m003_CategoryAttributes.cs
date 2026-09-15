using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechStore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class m003_CategoryAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryAttribute",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "String")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryAttribute_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttribute_CategoryId_Key",
                table: "CategoryAttribute",
                columns: new[] { "CategoryId", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryAttribute");
        }
    }
}
