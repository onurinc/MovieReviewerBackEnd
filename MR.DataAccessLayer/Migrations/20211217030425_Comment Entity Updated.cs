using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MR.DataAccessLayer.Migrations
{
    public partial class CommentEntityUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "CommentId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Comments",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                table: "Comments",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
