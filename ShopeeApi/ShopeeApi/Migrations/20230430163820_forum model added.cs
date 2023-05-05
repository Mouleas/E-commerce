using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopeeApi.Migrations
{
    /// <inheritdoc />
    public partial class forummodeladded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ForumModel",
                columns: table => new
                {
                    ForumId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ForumSubject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ForumBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ForumStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumModel", x => x.ForumId);
                    table.ForeignKey(
                        name: "FK_ForumModel_InventoryModel_ItemId",
                        column: x => x.ItemId,
                        principalTable: "InventoryModel",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumModel_UserModel_UserId",
                        column: x => x.UserId,
                        principalTable: "UserModel",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForumModel_ItemId",
                table: "ForumModel",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumModel_UserId",
                table: "ForumModel",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForumModel");
        }
    }
}
