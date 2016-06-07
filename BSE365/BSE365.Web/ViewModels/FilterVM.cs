using Newtonsoft.Json.Linq;
namespace BSE365.ViewModels
{
    public class FilterVM
    {
        public Pagination Pagination { get; set; }

        public Search Search { get; set; }
    }

    public class Pagination
    {
        public int Number { get; set; }

        public int Start { get; set; }

        public int TotalItemCount { get; set; }
    }

    public class Search
    {
        public JToken PredicateObject { get; set; }
    }
}