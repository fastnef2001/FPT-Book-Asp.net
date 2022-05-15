using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace important1.Migrations
{
    public partial class AddTotalPerCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
     
            migrationBuilder.AddColumn<int>(
                name: "TotalPerCart",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "TotalPerCart",
                table: "Carts");
        }
    }
}
