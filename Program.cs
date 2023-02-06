using HtmlAgilityPack;
using IMDB_Crawling.Data;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Phillips_Crawling_Task.Service;
using System;
using System.IO;

namespace IMDB_Crawling
{
    class Program
    {
        private static readonly IMDBContext _context = new();
        private const string Url = "https://www.imdb.com/";
        private const string BaseUrl = "https://www.imdb.com";
        static void Main(string[] args)
        {
            GetIMDBDetails();
            Console.ReadLine();
        }

        public static void GetFullyLoadedWebPage(WebDriver driver)
        {
            long scrollHeight = 0;
            IJavaScriptExecutor js = driver;
            do
            {
                var newScrollHeight = (long)js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight); return document.body.scrollHeight;");
                if (newScrollHeight != scrollHeight)
                {
                    scrollHeight = newScrollHeight;
                    Thread.Sleep(3000);
                }
                else
                {
                    Thread.Sleep(4000);
                    break;
                }
            } while (true);
        }

        public static string GetFullyLoadedWebPageContent(WebDriver driver)
        {
            long scrollHeight = 0;
            IJavaScriptExecutor js = driver;
            do
            {
                var newScrollHeight = (long)js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight); return document.body.scrollHeight;");
                if (newScrollHeight != scrollHeight)
                {
                    scrollHeight = newScrollHeight;
                    Thread.Sleep(3000);
                }
                else
                {
                    Thread.Sleep(4000);
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
            var pageSiurce = GetFullyLoadedWebPageContent(driver);
            var Details = new HtmlDocument();
            Details.LoadHtml(pageSiurce);
            var AllMovies = Details.DocumentNode.SelectNodes(XpathStrings.MovieListXpath);

            try
            {
                foreach (var movie in AllMovies)
                {
                    var movieUrl = BaseUrl + movie.SelectSingleNode(XpathStrings.SingleMovieLinkXpath).GetAttributes("href").First().Value;
                    var movieTitle = movie.SelectNodes(XpathStrings.MovieTitleXpath).First().InnerHtml.Trim() ?? string.Empty;
                    var movieLink = BaseUrl + movie.SelectNodes(XpathStrings.MovieLinkXpath).First().GetAttributes("href").First().Value ?? string.Empty;
                    var movieIMDB = movie.SelectNodes(XpathStrings.MovieIMDBXpath).First().InnerHtml.Trim() ?? string.Empty;
                    var movieId = RegexString.movieIdRegex.Match(movieLink).Groups[1].Value;
                    var movieRank = movie.SelectNodes(XpathStrings.MovieRankInIMDBXpath)?.First().GetAttributes("data-value")?.First().Value ?? string.Empty;

                    Console.WriteLine($"----------Movie with Id = {movieId}----------");
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
                    GetFullyLoadedWebPage(driver);

                    var movieReleaseYear = string.Empty;
                    var movieGenresString = string.Empty;
                    var movieWritersString = string.Empty;
                    var movieStarsString = string.Empty;
                    var movieDirectorsString = string.Empty;
                    var movieTimeDuration = string.Empty;
                    var movieIMDBVoterCount = string.Empty;
                    var movieDescription = string.Empty;
                    var moviePosterImageUrl = string.Empty;
                    var movieWatchOnPrimeLink = string.Empty;

                    try { movieReleaseYear = driver.FindElement(By.XPath(XpathStrings.MovieReleaseYearXpath)).Text; }
                    catch (Exception) { movieReleaseYear = string.Empty; }

                    try { movieTimeDuration = driver.FindElement(By.XPath(XpathStrings.MovieTimeDurationXpath))?.Text; }
                    catch (Exception) { movieTimeDuration = string.Empty; }

                    try { movieIMDBVoterCount = driver.FindElement(By.XPath(XpathStrings.MovieIMDBVoterCountXpath))?.Text; }
                    catch (Exception) { movieIMDBVoterCount = string.Empty; }

                    try { movieDescription = driver.FindElement(By.XPath(XpathStrings.MovieDescriptionXpath))?.Text; }
                    catch (Exception) { movieDescription = string.Empty; }

                    try { moviePosterImageUrl = driver.FindElement(By.XPath(XpathStrings.MoviePosterImageUrlXpath))?.GetAttribute("src"); }
                    catch (Exception) { moviePosterImageUrl = string.Empty; }

                    try { movieWatchOnPrimeLink = driver.FindElement(By.XPath(XpathStrings.MovieWatchOnPrimeLink))?.GetAttribute("href"); }
                    catch (Exception) { movieWatchOnPrimeLink = string.Empty; }

                    var movieGenres = driver.FindElements(By.XPath(XpathStrings.MovieGenresXpath));
                    var movieWriters = driver.FindElements(By.XPath(XpathStrings.MovieWritersXpath));
                    var movieStars = driver.FindElements(By.XPath(XpathStrings.MovieStarsXpath));
                    var movieDirectors = driver.FindElements(By.XPath(XpathStrings.MovieDirectorXpath));

                    if (movieDirectors != null)
                        foreach (var director in movieDirectors)
                            if (movieDirectors!.IndexOf(director) != movieDirectors.Count - 1)
                                movieDirectorsString += director.Text + ", ";
                            else
                                movieDirectorsString += director.Text;

                    if (movieGenres != null)
                        foreach (var genres in movieGenres)
                            if (movieGenres.IndexOf(genres) != movieGenres.Count - 1)
                                movieGenresString += genres.Text + ", ";
                            else
                                movieGenresString += genres.Text;

                    if (movieWriters != null)
                        foreach (var writer in movieWriters)
                            if (movieWriters.IndexOf(writer) != movieWriters.Count - 1)
                                movieWritersString += writer.Text + ", ";
                            else
                                movieWritersString += writer.Text;

                    if (movieStars != null)
                        foreach (var star in movieStars)
                            if (movieStars.IndexOf(star) != movieStars.Count - 1)
                                movieStarsString += star.Text + ", ";
                            else
                                movieStarsString += star.Text;

                    Console.WriteLine($"----------Movie Details of Movie Id = {movieId}----------");
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
                    Console.WriteLine();

                    var movieRecord = await _context.tbl_Movie_Details.Where(x => x.MovieId == movieId).FirstOrDefaultAsync();
                    if (movieRecord != null)
                    {
                        movieRecord.Id = movieRecord.Id;
                        movieRecord.MovieId = movieId;
                        movieRecord.ReleaseYear = movieReleaseYear;
                        movieRecord.TimeDuration = movieTimeDuration!;
                        movieRecord.PosterImageUrl = moviePosterImageUrl!;
                        movieRecord.Genres = movieGenresString;
                        movieRecord.Description = movieDescription!;
                        movieRecord.Director = movieDirectorsString;
                        movieRecord.Writer = movieWritersString;
                        movieRecord.Stars = movieStarsString;
                        movieRecord.WatchOnPrimeLink = movieWatchOnPrimeLink!;
                        _context.tbl_Movie_Details.Update(movieRecord);
                    }
                    else
                    {
                        MovieDetails movieDetails = new()
                        {
                            MovieId = movieId,
                            ReleaseYear = movieReleaseYear,
                            TimeDuration = movieTimeDuration!,
                            IMDBVoterCount = movieIMDBVoterCount!,
                            PosterImageUrl = moviePosterImageUrl!,
                            Genres = movieGenresString,
                            Description = movieDescription!,
                            Director = movieDirectorsString,
                            Writer = movieWritersString,
                            Stars = movieStarsString,
                            WatchOnPrimeLink = movieWatchOnPrimeLink!
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
                driver.Close();
                Console.WriteLine("Data Saved Successfully...!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}