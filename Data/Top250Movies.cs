using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDB_Crawling.Data
{
    public class Top250Movies
    {
        [Key]
        public string Id { get; set; }
        public int MovieRankInIMDB { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string IMDB { get; set; }
    }
}
