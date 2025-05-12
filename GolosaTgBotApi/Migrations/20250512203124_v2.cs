using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolosaTgBotApi.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_LinkedChat_ChatId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkedChat_Channels_ChannelID",
                table: "LinkedChat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LinkedChat",
                table: "LinkedChat");

            migrationBuilder.RenameTable(
                name: "LinkedChat",
                newName: "linkedchat");

            migrationBuilder.RenameIndex(
                name: "IX_LinkedChat_ChannelID",
                table: "linkedchat",
                newName: "IX_linkedchat_ChannelID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_linkedchat",
                table: "linkedchat",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_linkedchat_ChatId",
                table: "Comments",
                column: "ChatId",
                principalTable: "linkedchat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_linkedchat_Channels_ChannelID",
                table: "linkedchat",
                column: "ChannelID",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_linkedchat_ChatId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_linkedchat_Channels_ChannelID",
                table: "linkedchat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_linkedchat",
                table: "linkedchat");

            migrationBuilder.RenameTable(
                name: "linkedchat",
                newName: "LinkedChat");

            migrationBuilder.RenameIndex(
                name: "IX_linkedchat_ChannelID",
                table: "LinkedChat",
                newName: "IX_LinkedChat_ChannelID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LinkedChat",
                table: "LinkedChat",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_LinkedChat_ChatId",
                table: "Comments",
                column: "ChatId",
                principalTable: "LinkedChat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedChat_Channels_ChannelID",
                table: "LinkedChat",
                column: "ChannelID",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
