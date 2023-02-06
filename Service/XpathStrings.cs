using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phillips_Crawling_Task.Service
{
    public class XpathStrings
    {
        public static readonly string Top250MoviesXpath = "//span[.='Top 250 Movies']";
        public static readonly string MovieListXpath = "//tbody[@class='lister-list']/tr";
        public static readonly string MovieRankInIMDBXpath = ".//td/span[@name='rk']";
        public static readonly string SingleMovieLinkXpath = ".//td/a";
        public static readonly string MovieLinkXpath = ".//td[@class='posterColumn']/a";
        public static readonly string MovieTitleXpath = ".//td[@class='titleColumn']/a";
        public static readonly string MovieIMDBXpath = ".//td[@class='ratingColumn imdbRating']/strong";
        public static readonly string MovieIMDBVoterCountXpath = "(//div[@class='sc-7ab21ed2-3 iDwwZL'])[1]";
        public static readonly string MovieReleaseYearXpath = "(//a[@class='ipc-link ipc-link--baseAlt ipc-link--inherit-color sc-8c396aa2-1 iOtMms'])[1]";
        public static readonly string MovieTimeDurationXpath = "(//ul[@class='ipc-inline-list ipc-inline-list--show-dividers sc-8c396aa2-0 bwvZbJ baseAlt']/li)[3]";
        public static readonly string MoviePosterImageUrlXpath = "//div[@class='ipc-media ipc-media--poster-27x40 ipc-image-media-ratio--poster-27x40 ipc-media--baseAlt ipc-media--poster-l ipc-poster__poster-image ipc-media__img']/img";
        public static readonly string MovieGenresXpath = "//a[@class='sc-b5a9e5a3-3 cDXGWB ipc-chip ipc-chip--on-baseAlt']/span";
        public static readonly string MovieDescriptionXpath = "//span[@class='sc-b5a9e5a3-2 jTXFcx']";
        public static readonly string MovieDirectorXpath = "(//*[contains(text(),'Director')]/following-sibling::div/ul)[1]/li";
        public static readonly string MovieWritersXpath = "(//*[contains(text(),'Writer')]/following-sibling::div/ul)[1]/li";
        public static readonly string MovieStarsXpath = "(//*[contains(text(),'Star')]/following-sibling::div/ul)[1]/li";
        public static readonly string MovieWatchOnPrimeLink = "//a[@class='ipc-btn ipc-btn--full-width ipc-btn--center-align-content ipc-btn--large-height ipc-btn--core-accent1 ipc-btn--theme-baseAlt sc-3b14d2c8-0 bmaqfn']";
    }
}
