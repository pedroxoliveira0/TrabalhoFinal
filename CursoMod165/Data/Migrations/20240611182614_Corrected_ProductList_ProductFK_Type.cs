using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CursoMod165.Data.Migrations
{
    /// <inheritdoc />
    public partial class Corrected_ProductList_ProductFK_Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductLists_Customers_ProductID",
                table: "ProductLists");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLists_Products_ProductID",
                table: "ProductLists",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductLists_Products_ProductID",
                table: "ProductLists");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLists_Customers_ProductID",
                table: "ProductLists",
                column: "ProductID",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
