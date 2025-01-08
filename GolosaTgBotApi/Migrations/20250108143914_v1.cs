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
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Channels_ChannelId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ChannelId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChannelId",
                table: "Comments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ChannelId",
                table: "Comments",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Channels_ChannelId",
                table: "Comments",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id");
        }
    }
}
