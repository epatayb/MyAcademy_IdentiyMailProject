using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentiyMail.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageTrashRetentionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedByReceiverAt",
                table: "UserMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedBySenderAt",
                table: "UserMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPermanentlyDeletedByReceiver",
                table: "UserMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPermanentlyDeletedBySender",
                table: "UserMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedByReceiverAt",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "DeletedBySenderAt",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "IsPermanentlyDeletedByReceiver",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "IsPermanentlyDeletedBySender",
                table: "UserMessages");
        }
    }
}
