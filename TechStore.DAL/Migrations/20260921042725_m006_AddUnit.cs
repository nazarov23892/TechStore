using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TechStore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class m006_AddUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UnitId",
                table: "CategoryAttribute",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1L, "м" },
                    { 2L, "см" },
                    { 3L, "мм" },
                    { 4L, "г" },
                    { 5L, "кг" },
                    { 6L, "Б" },
                    { 7L, "КБ" },
                    { 8L, "МБ" },
                    { 9L, "МГц" },
                    { 10L, "КГц" },
                    { 11L, "Вт" },
                    { 12L, "шт" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttribute_UnitId",
                table: "CategoryAttribute",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_Name",
                table: "Units",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryAttribute_Units_UnitId",
                table: "CategoryAttribute",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryAttribute_Units_UnitId",
                table: "CategoryAttribute");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropIndex(
                name: "IX_CategoryAttribute_UnitId",
                table: "CategoryAttribute");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "CategoryAttribute");
        }
    }
}
