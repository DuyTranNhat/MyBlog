using System;
using Bogus;
using Microsoft.EntityFrameworkCore.Migrations;
using Migration_EF.Models;

#nullable disable

namespace Migration_EF.Migrations
{
    /// <inheritdoc />
    public partial class v0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.ID);
                });

            //migrationBuilder.InsertData(
            //    table: "Article", 
            //    columns: new[] { "Title", "PublishDate", "Content" },
            //    values: new object[] { "Bai viet 1", new DateTime(2022, 8, 12), "Nội Dung 1" } );

            Randomizer.Seed = new Random(8675309);
            var fakerArticle = new Faker<Article>();
            //Phat sinh ra cau van co 5 -> 10 từ
            fakerArticle.RuleFor(a => a.Title, fakerArticle => fakerArticle.Lorem.Sentence(5, 5));
            fakerArticle.RuleFor(a => a.PublishDate, fakerArticle => fakerArticle.Date.Between(new DateTime(2023, 1, 1), new DateTime(2024, 7, 30)));
            fakerArticle.RuleFor(a => a.Content, fakerArticle => fakerArticle.Lorem.Paragraph(1));

            for (int i = 0; i < 150; i++)
            {
                Article article = fakerArticle.Generate();
                migrationBuilder.InsertData(
                    table: "Article",
                    columns: new[] { "Title", "PublishDate", "Content" },
                    values: new object[] { article.Title, article.PublishDate, article.Content });
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Article");
        }
    }
}
