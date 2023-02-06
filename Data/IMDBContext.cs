using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDB_Crawling.Data
{
    public class IMDBContext : DbContext
    {
        public IMDBContext()
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"data source=DESKTOP-9J2CV47; database=IMDB; integrated security=SSPI");
        }

        public DbSet<Top250Movies> tbl_Top_250_Movies { get; set; }
        public DbSet<MovieDetails> tbl_Movie_Details { get; set; }
    }
}
