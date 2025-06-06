﻿using Microsoft.EntityFrameworkCore.Migrations;

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
                name: "OrderBySlots",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    SpentSlot = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    OwnerPkh = table.Column<string>(type: "text", nullable: false),
                    RawCbor = table.Column<byte[]>(type: "bytea", nullable: false),
                    RawDatum = table.Column<byte[]>(type: "bytea", nullable: false),
                    PolicyId = table.Column<string>(type: "text", nullable: false),
                    AssetName = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    OrderStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBySlots", x => new { x.Id, x.Index });
                });

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
                name: "OrderBySlots",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ReducerStates",
                schema: "public");
        }
    }
}
