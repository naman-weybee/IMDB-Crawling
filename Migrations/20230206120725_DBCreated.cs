using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMDB_Crawling.Migrations
{
    public partial class DBCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_Top_250_Movies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MovieRankInIMDB = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IMDB = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Top_250_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Movie_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReleaseYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeDuration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IMDBVoterCount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PosterImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Director = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Writer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stars = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WatchOnPrimeLink = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Movie_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Movie_Details_tbl_Top_250_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "tbl_Top_250_Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Movie_Details_MovieId",
                table: "tbl_Movie_Details",
                column: "MovieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_Movie_Details");

            migrationBuilder.DropTable(
                name: "tbl_Top_250_Movies");
        }
    }
}
