using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolosaTgBotApi.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "Users",
                newName: "AvatarFileId");

            migrationBuilder.AddColumn<string>(
                name: "PhotosFileId",
                table: "Posts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotosFileId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "AvatarFileId",
                table: "Users",
                newName: "AvatarUrl");
        }
    }
}
