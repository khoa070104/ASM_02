using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FUNewsManagement.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoryDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ParentCategoryId = table.Column<short>(type: "smallint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_Category_Category_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemAccounts",
                columns: table => new
                {
                    AccountID = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountRole = table.Column<short>(type: "smallint", nullable: false),
                    AccountPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAccounts", x => x.AccountID);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "NewsArticle",
                columns: table => new
                {
                    NewsArticleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NewsTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Headline = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NewsContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewsSource = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CategoryId = table.Column<short>(type: "smallint", nullable: false),
                    NewsStatus = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<short>(type: "smallint", nullable: false),
                    UpdatedById = table.Column<short>(type: "smallint", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticle", x => x.NewsArticleId);
                    table.ForeignKey(
                        name: "FK_NewsArticle_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewsArticle_SystemAccounts_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "SystemAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewsArticle_SystemAccounts_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "SystemAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewsTag",
                columns: table => new
                {
                    NewsArticleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsTag", x => new { x.NewsArticleId, x.TagId });
                    table.ForeignKey(
                        name: "FK_NewsTag_NewsArticle_NewsArticleId",
                        column: x => x.NewsArticleId,
                        principalTable: "NewsArticle",
                        principalColumn: "NewsArticleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "CategoryId", "CategoryDescription", "CategoryName", "IsActive", "ParentCategoryId" },
                values: new object[,]
                {
                    { (short)1, "General news and announcements", "General News", true, null },
                    { (short)2, "Academic related news", "Academic", true, null },
                    { (short)3, "University events and activities", "Events", true, null }
                });

            migrationBuilder.InsertData(
                table: "SystemAccounts",
                columns: new[] { "AccountID", "AccountEmail", "AccountName", "AccountPassword", "AccountRole" },
                values: new object[] { (short)1, "admin@FUNewsManagementSystem.org", "Administrator", "@@abc123@@", (short)0 });

            migrationBuilder.CreateIndex(
                name: "IX_Category_ParentCategoryId",
                table: "Category",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticle_CategoryId",
                table: "NewsArticle",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticle_CreatedById",
                table: "NewsArticle",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticle_UpdatedById",
                table: "NewsArticle",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NewsTag_TagId",
                table: "NewsTag",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsTag");

            migrationBuilder.DropTable(
                name: "NewsArticle");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "SystemAccounts");
        }
    }
}
