using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Argus.Sync.Example.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "ReducerStates",
                schema: "public",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReducerStates", x => new { x.Name, x.Slot });
                });

            migrationBuilder.CreateTable(
                name: "TxOutputBySlot",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<long>(type: "bigint", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    RawCbor = table.Column<byte[]>(type: "bytea", nullable: false),
                    DatumData = table.Column<byte[]>(type: "bytea", nullable: false),
                    DatumType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TxOutputBySlot", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReducerStates_Name_Slot",
                schema: "public",
                table: "ReducerStates",
                columns: new[] { "Name", "Slot" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_ReducerStates_Slot",
                schema: "public",
                table: "ReducerStates",
                column: "Slot",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReducerStates",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TxOutputBySlot",
                schema: "public");
        }
    }
}
