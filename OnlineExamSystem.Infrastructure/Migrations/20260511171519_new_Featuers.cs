using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineExamSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class new_Featuers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Choices_ChoiceId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_ChoiceId",
                table: "UserAnswers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d4c1fa52-9a2e-47b6-9cb1-34a6d612c8e7");

            migrationBuilder.DropColumn(
                name: "ChoiceId",
                table: "UserAnswers");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AuditLogs",
                newName: "Timestamp");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "e4c1fa52-9a2e-47b6-9cb1-34a6d612c8e7", null, "Teacher", "TEACHER" },
                    { "f5d2fb63-0b3f-58c7-0dc2-45b7e723d9f8", null, "Student", "STUDENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e4c1fa52-9a2e-47b6-9cb1-34a6d612c8e7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f5d2fb63-0b3f-58c7-0dc2-45b7e723d9f8");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "AuditLogs",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<int>(
                name: "ChoiceId",
                table: "UserAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d4c1fa52-9a2e-47b6-9cb1-34a6d612c8e7", null, "User", "USER" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_ChoiceId",
                table: "UserAnswers",
                column: "ChoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Choices_ChoiceId",
                table: "UserAnswers",
                column: "ChoiceId",
                principalTable: "Choices",
                principalColumn: "ChoiceId");
        }
    }
}
