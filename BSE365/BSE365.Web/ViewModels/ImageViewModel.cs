
namespace BSE365.ViewModels
{
    public class ImageViewModel
    {
        public int Id { get; set; }

        public byte[] Content { get; set; }

        public string Extension { get; set; }

        public string Url
        {
            get
            {
                return "/image/getUserPicture/" + Id;
            }
        }
    }
}