using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace important1.Migrations
{
    public partial class AddCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    UId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookIsbn = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => new { x.UId, x.BookIsbn });
                    table.ForeignKey(
                        name: "FK_Carts_AspNetUsers_UId",
                        column: x => x.UId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Carts_Books_BookIsbn",
                        column: x => x.BookIsbn,
                        principalTable: "Books",
                        principalColumn: "Isbn");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_BookIsbn",
                table: "Carts",
                column: "BookIsbn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Carts");
        }
    }
}
