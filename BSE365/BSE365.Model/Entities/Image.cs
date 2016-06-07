namespace BSE365.Model.Entities
{
    public class Image : BaseEntity
    {
        public string Extension { get; set; }

        public byte[] Content { get; set; }
    }
}