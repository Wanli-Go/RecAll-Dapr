﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecAll.Core.List.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "list");

            migrationBuilder.CreateSequence(
                name: "listseq",
                schema: "list",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "setseq",
                schema: "list",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "listtypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listtypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserIdentityGuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lists_listtypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "listtypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "sets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    UserIdentityGuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ListId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sets_lists_ListId",
                        column: x => x.ListId,
                        principalTable: "lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sets_listtypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "listtypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_lists_TypeId",
                table: "lists",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_sets_ListId",
                table: "sets",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_sets_TypeId",
                table: "sets",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sets");

            migrationBuilder.DropTable(
                name: "lists");

            migrationBuilder.DropTable(
                name: "listtypes");

            migrationBuilder.DropSequence(
                name: "listseq",
                schema: "list");

            migrationBuilder.DropSequence(
                name: "setseq",
                schema: "list");
        }
    }
}
