namespace BSE365.ViewModels
{
    public class ImageViewModel
    {
        public int Id { get; set; }

        public byte[] Content { get; set; }

        public string Extension { get; set; }

        public string Url
        {
            get { return ImageUrl + Id; }
        }

        public static string ImageUrl = @"/image/getUserPicture/";
    }
}