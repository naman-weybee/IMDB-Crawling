using HtmlAgilityPack;
using IMDB_Crawling.Data;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Phillips_Crawling_Task.Service;
using System.IO;

namespace IMDB_Crawling
{
    class Program
    {
        private static readonly IMDBContext _context = new();
        private const string Url = "https://www.imdb.com/";
        static void Main(string[] args)
        {
            GetIMDBDetails();
            Console.ReadLine();
        }

        public static void GetFullyLoadedWebPageContent(WebDriver driver)
        {
            long scrollHeight = 0;
            IJavaScriptExecutor js = driver;
            do
            {
                var newScrollHeight = (long)js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight); return document.body.scrollHeight;");
                if (newScrollHeight != scrollHeight)
                {
                    scrollHeight = newScrollHeight;
                    Thread.Sleep(1500);
                }
                else
                {
                    Thread.Sleep(2500);
                    break;
                }
            } while (true);
        }

        public static string GetFullyLoadedPageContent(WebDriver driver)
        {
            long scrollHeight = 0;
            IJavaScriptExecutor js = driver;
            do
            {
                var newScrollHeight = (long)js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight); return document.body.scrollHeight;");
                if (newScrollHeight != scrollHeight)
                {
                    scrollHeight = newScrollHeight;
                    Thread.Sleep(1500);
                }
                else
                {
                    Thread.Sleep(2500);
                    break;
                }
            } while (true);
            return driver.PageSource;
        }

        private static async void GetIMDBDetails()
        {
            ChromeOptions opt = new();
            opt.AddArgument("--log-level=3");
            opt.AddArguments("--disable-gpu");
            opt.AddArguments("--start-maximized");
            opt.AddArguments("--no-sandbox");

            ChromeDriver driver = new(ChromeDriverService.CreateDefaultService(), opt, TimeSpan.FromMinutes(3));
            driver.Navigate().GoToUrl(Url);

            driver.FindElement(By.Id("imdbHeader-navDrawerOpen")).Click();
            Thread.Sleep(500);

            driver.FindElement(By.XPath(XpathStrings.Top250MoviesXpath)).Click();
            var pageSiurce = GetFullyLoadedPageContent(driver);
            var Details = new HtmlDocument();
            Details.LoadHtml(pageSiurce);
            var AllMovies = Details.DocumentNode.SelectNodes(XpathStrings.MovieListXpath);

            try
            {
                foreach (var movie in AllMovies)
                {
                    var movieUrl = "https://www.imdb.com/" + movie.SelectSingleNode(XpathStrings.SingleMovieLinkXpath).GetAttributes("href").First().Value;
                    var movieTitle = movie.SelectNodes(XpathStrings.MovieTitleXpath).First().InnerHtml.Trim() ?? string.Empty;
                    var movieLink = movie.SelectNodes(XpathStrings.MovieLinkXpath).First().GetAttributes("href").First().Value ?? string.Empty;
                    var movieIMDB = movie.SelectNodes(XpathStrings.MovieIMDBXpath).First().InnerHtml.Trim() ?? string.Empty;
                    var movieId = RegexString.movieIdRegex.Match(movieLink).Groups[1].Value;
                    var movieRank = AllMovies.IndexOf(movie) + 1;

                    Console.WriteLine($"Movie Rank in IMDB: {movieRank}");
                    Console.WriteLine($"Movie Id: {movieId}");
                    Console.WriteLine($"Movie Title: {movieTitle}");
                    Console.WriteLine($"Movie IMDB: {movieIMDB}");
                    Console.WriteLine($"Movie Link: {movieLink}");
                    Console.WriteLine();

                    var topMovieRecord = await _context.tbl_Top_250_Movies.Where(x => x.Id == movieId).FirstOrDefaultAsync();
                    if (topMovieRecord != null)
                    {
                        topMovieRecord.Id = movieId;
                        topMovieRecord.MovieRankInIMDB = movieRank;
                        topMovieRecord.Title = movieTitle;
                        topMovieRecord.Link = movieLink;
                        topMovieRecord.IMDB = movieIMDB;
                        _context.tbl_Top_250_Movies.Update(topMovieRecord);
                    }
                    else
                    {
                        Top250Movies top250Movies = new()
                        {
                            Id = movieId,
                            MovieRankInIMDB = movieRank,
                            Title = movieTitle,
                            IMDB = movieIMDB,
                            Link = movieLink
                        };
                        await _context.tbl_Top_250_Movies.AddAsync(top250Movies);
                    }

                    driver.Navigate().GoToUrl(movieUrl);
                    GetFullyLoadedWebPageContent(driver);

                    var movieReleaseYear = driver.FindElement(By.XPath(XpathStrings.MovieReleaseYearXpath)).Text ?? string.Empty;
                    var movieTimeDuration = driver.FindElement(By.XPath(XpathStrings.MovieTimeDurationXpath)).Text ?? string.Empty;
                    var movieIMDBVoterCount = driver.FindElement(By.XPath(XpathStrings.MovieIMDBVoterCountXpath)).Text ?? string.Empty;
                    var movieDescription = driver.FindElement(By.XPath(XpathStrings.MovieDescriptionXpath)).Text ?? string.Empty;
                    var moviePosterImageUrl = driver.FindElement(By.XPath(XpathStrings.MoviePosterImageUrlXpath)).GetAttribute("src") ?? string.Empty;
                    var movieWatchOnPrimeLink = driver.FindElement(By.XPath(XpathStrings.MovieWatchOnPrimeLink)).GetAttribute("href") ?? string.Empty;
                    var movieGenres = driver.FindElements(By.XPath(XpathStrings.MovieGenresXpath)).ToList();
                    var movieWriters = driver.FindElements(By.XPath(XpathStrings.MovieWritersXpath)).ToList();
                    var movieStars = driver.FindElements(By.XPath(XpathStrings.MovieStarsXpath)).ToList();
                    var movieDirectors = driver.FindElements(By.XPath(XpathStrings.MovieDirectorXpath)).ToList();
                    var movieGenresString = string.Empty;
                    var movieWritersString = string.Empty;
                    var movieStarsString = string.Empty;
                    var movieDirectorsString = string.Empty;

                    foreach (var director in movieDirectors)
                        if (movieGenres.IndexOf(director) != movieGenres.Count)
                            movieDirectorsString += director.Text + ",";
                        else
                            movieDirectorsString += director.Text;

                    foreach (var genres in movieGenres)
                        if (movieGenres.IndexOf(genres) != movieGenres.Count)
                            movieGenresString += genres.Text + ",";
                        else
                            movieGenresString += genres.Text;

                    foreach (var writer in movieWriters)
                        if (movieWriters.IndexOf(writer) != movieWriters.Count)
                            movieWritersString += writer.Text + ",";
                        else
                            movieWritersString += writer.Text;

                    foreach (var star in movieStars)
                        if (movieWriters.IndexOf(star) != movieWriters.Count)
                            movieStarsString += star.Text + ",";
                        else
                            movieStarsString += star.Text;

                    Console.WriteLine($"Movie Id: {movieId}");
                    Console.WriteLine($"Release Year: {movieReleaseYear}");
                    Console.WriteLine($"Time Duration: {movieTimeDuration}");
                    Console.WriteLine($"Poster Image Url: {moviePosterImageUrl}");
                    Console.WriteLine($"Genres: {movieGenresString}");
                    Console.WriteLine($"Description: {movieDescription}");
                    Console.WriteLine($"Director: {movieDirectorsString}");
                    Console.WriteLine($"Writer: {movieWritersString}");
                    Console.WriteLine($"Stars: {movieStarsString}");
                    Console.WriteLine($"Watch On Prime Link: {movieWatchOnPrimeLink}");

                    var movieRecord = await _context.tbl_Movie_Details.Where(x => x.MovieId == movieId).FirstOrDefaultAsync();
                    if (movieRecord != null)
                    {
                        movieRecord.Id = movieRecord.Id;
                        movieRecord.MovieId = movieId;
                        movieRecord.ReleaseYear = movieReleaseYear;
                        movieRecord.TimeDuration = movieTimeDuration;
                        movieRecord.PosterImageUrl = moviePosterImageUrl;
                        movieRecord.Genres = movieGenresString;
                        movieRecord.Description = movieDescription;
                        movieRecord.Director = movieDirectorsString;
                        movieRecord.Writer = movieWritersString;
                        movieRecord.Stars = movieStarsString;
                        movieRecord.WatchOnPrimeLink = movieWatchOnPrimeLink;
                        _context.tbl_Movie_Details.Update(movieRecord);
                    }
                    else
                    {
                        MovieDetails movieDetails = new()
                        {
                            MovieId = movieId,
                            ReleaseYear = movieReleaseYear,
                            TimeDuration = movieTimeDuration,
                            IMDBVoterCount = movieIMDBVoterCount,
                            PosterImageUrl = moviePosterImageUrl,
                            Genres = movieGenresString,
                            Description = movieDescription,
                            Director = movieDirectorsString,
                            Writer = movieWritersString,
                            Stars = movieStarsString,
                            WatchOnPrimeLink = movieWatchOnPrimeLink
                        };
                        await _context.tbl_Movie_Details.AddAsync(movieDetails);
                    }
                    await _context.SaveChangesAsync();
                    Console.WriteLine();
                    Console.WriteLine("=====================================================================================================================");
                    Console.WriteLine($"Data Saved in Top250Movies and MovieDetails Tables with Movie Id: {movieId}");
                    Console.WriteLine("=====================================================================================================================");
                    Console.WriteLine();
                }
                Console.WriteLine("Data Saved Successfully...!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}