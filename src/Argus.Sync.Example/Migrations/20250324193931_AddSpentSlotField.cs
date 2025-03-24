using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Argus.Sync.Example.Migrations
{
    /// <inheritdoc />
    public partial class AddSpentSlotField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatumData",
                schema: "public",
                table: "TxOutputBySlot");

            migrationBuilder.DropColumn(
                name: "DatumType",
                schema: "public",
                table: "TxOutputBySlot");

            migrationBuilder.AlterColumn<decimal>(
                name: "Index",
                schema: "public",
                table: "TxOutputBySlot",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<decimal>(
                name: "SpentSlot",
                schema: "public",
                table: "TxOutputBySlot",
                type: "numeric(20,0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpentSlot",
                schema: "public",
                table: "TxOutputBySlot");

            migrationBuilder.AlterColumn<long>(
                name: "Index",
                schema: "public",
                table: "TxOutputBySlot",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AddColumn<byte[]>(
                name: "DatumData",
                schema: "public",
                table: "TxOutputBySlot",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "DatumType",
                schema: "public",
                table: "TxOutputBySlot",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
