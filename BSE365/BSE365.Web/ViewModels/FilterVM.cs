using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.ViewModels
{
    public class FilterVM
    {
        public string Summary { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }

        public string Order { get; set; }
        public bool OrderReverse { get; set; }
    }
}