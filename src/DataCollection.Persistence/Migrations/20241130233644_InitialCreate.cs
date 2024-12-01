using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataCollection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WindowsData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WindowTitle = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ProcessFileName = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ProcessFriendlyName = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    StartTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    StopTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WindowsData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WindowsData");
        }
    }
}
