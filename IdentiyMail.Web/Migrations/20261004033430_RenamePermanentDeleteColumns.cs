using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentiyMail.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenamePermanentDeleteColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPermanetlyDeletedByReceiver",
                table: "UserMessages",
                newName: "IsPermanentlyDeletedByReceiver");

            migrationBuilder.RenameColumn(
                name: "IsPermanetlyDeletedBySender",
                table: "UserMessages",
                newName: "IsPermanentlyDeletedBySender");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPermanentlyDeletedByReceiver",
                table: "UserMessages",
                newName: "IsPermanetlyDeletedByReceiver");

            migrationBuilder.RenameColumn(
                name: "IsPermanentlyDeletedBySender",
                table: "UserMessages",
                newName: "IsPermanetlyDeletedBySender");
        }
    }
}
