using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.ViewModels
{
    public class ResultViewModel<T>
    {
        public T Result { get; set; }

        public bool IsSuccessful { get; set; }
    }
}