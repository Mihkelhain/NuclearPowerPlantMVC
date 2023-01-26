using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuclearPowerPlantMVC.Migrations
{
    public partial class fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTurnedOn",
                table: "Reactors");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTurnedOn",
                table: "NuclearPlants",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTurnedOn",
                table: "NuclearPlants");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTurnedOn",
                table: "Reactors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
