using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace lawhands.Data.Migrations
{
    public partial class rating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rating",
                table: "Movie",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Movie",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "Movie",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Movie");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Movie",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "Movie",
                nullable: true);
        }
    }
}
