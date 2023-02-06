using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Phillips_Crawling_Task.Service
{
    public class RegexString
    {
        public static readonly Regex movieIdRegex = new(@"/title/(\w+)", RegexOptions.IgnoreCase);
    }
}
