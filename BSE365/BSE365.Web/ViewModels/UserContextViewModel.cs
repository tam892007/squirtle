
using System.Collections.Generic;
namespace BSE365.ViewModels
{
    public class UserContextViewModel
    {
        public string Id { get; set; }

        public int PinBalance { get; set; }

        public string UserName { get; set; }

        public ImageViewModel Avatar { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}