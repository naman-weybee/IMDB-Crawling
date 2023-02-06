using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Phillips_Crawling_Task.Service;

namespace IMDB_Crawling
{
    class Program
    {
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

        private static void GetIMDBDetails()
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
            foreach (var movie in movieList)
            {
                var movieTitle = movie.FindElement(By.XPath(XpathStrings.MovieTitleXpath)).Text;
                var movieLink = movie.FindElement(By.XPath(XpathStrings.MovieLinkXpath)).GetAttribute("href");
                var movieIMDB = movie.FindElement(By.XPath(XpathStrings.MovieIMDBXpath)).Text;
                var movieId = RegexString.movieIdRegex.Match(movieLink).Groups[1].Value;

                Console.WriteLine("======================================================================================================");
                Console.WriteLine($"Movie Rank in IMDB: {movieList.IndexOf(movie) + 1}");
                Console.WriteLine($"Movie Id: {movieId}");
                Console.WriteLine($"Movie Title: {movieTitle}");
                Console.WriteLine($"Movie IMDB: {movieIMDB}");
                Console.WriteLine($"Movie Link: {movieLink}");
                Console.WriteLine();
            }
        }
    }
}