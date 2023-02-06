using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDB_Crawling.Data
{
    public class MovieDetails
    {
        [Key]
        public int Id { get; set; }
        public Top250Movies Movie { get; set; }
        public string MovieId { get; set; }
        public string ReleaseYear { get; set; }
        public string TimeDuration { get; set; }
        public string IMDBVoterCount { get; set; }
        public string PosterImageUrl { get; set; }
        public string Genres { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Stars { get; set; }
        public string WatchOnPrimeLink { get; set; }
    }
}
