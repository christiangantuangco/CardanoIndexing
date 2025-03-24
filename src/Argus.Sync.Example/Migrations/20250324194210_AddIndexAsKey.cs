using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Argus.Sync.Example.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexAsKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TxOutputBySlot",
                schema: "public",
                table: "TxOutputBySlot");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TxOutputBySlot",
                schema: "public",
                table: "TxOutputBySlot",
                columns: new[] { "Id", "Index" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TxOutputBySlot",
                schema: "public",
                table: "TxOutputBySlot");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TxOutputBySlot",
                schema: "public",
                table: "TxOutputBySlot",
                column: "Id");
        }
    }
}
