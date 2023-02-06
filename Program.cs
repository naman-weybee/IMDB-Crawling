using IMDB_Crawling.Data;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Phillips_Crawling_Task.Service;

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
            GetFullyLoadedWebPageContent(driver);

            IList<IWebElement> movieList = driver.FindElements(By.XPath(XpathStrings.MovieListXpath));
            try
            {
                foreach (var movie in movieList)
                {
                    var movieTitle = movie.FindElement(By.XPath(XpathStrings.MovieTitleXpath)).Text;
                    var movieLink = movie.FindElement(By.XPath(XpathStrings.MovieLinkXpath)).GetAttribute("href");
                    var movieIMDB = movie.FindElement(By.XPath(XpathStrings.MovieIMDBXpath)).Text;
                    var movieId = RegexString.movieIdRegex.Match(movieLink).Groups[1].Value;
                    var movieRank = movieList.IndexOf(movie) + 1;

                    Console.WriteLine("======================================================================================================");
                    Console.WriteLine($"Movie Rank in IMDB: {movieRank}");
                    Console.WriteLine($"Movie Id: {movieId}");
                    Console.WriteLine($"Movie Title: {movieTitle}");
                    Console.WriteLine($"Movie IMDB: {movieIMDB}");
                    Console.WriteLine($"Movie Link: {movieLink}");
                    Console.WriteLine();

                    var movieRecord = await _context.tbl_Top_250_Movies.Where(x => x.Id == movieId).FirstOrDefaultAsync();

                    if (movieRecord != null)
                    {
                        movieRecord.Id = movieId;
                        movieRecord.MovieRankInIMDB = movieRank;
                        movieRecord.Title = movieTitle;
                        movieRecord.Link = movieLink;
                        movieRecord.IMDB = movieIMDB;
                        _context.tbl_Top_250_Movies.Update(movieRecord);
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
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Data Saved Successfully...!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}