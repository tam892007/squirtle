namespace BSE365.ViewModels
{
    public class UserInfoViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string ParentId { get; set; }

        public string ParentName { get; set; }

        public string PhoneNumber { get; set; }

        public string BankNumber { get; set; }

        public string BankName { get; set; }

        public string BankBranch { get; set; }

        public ImageViewModel Avatar { get; set; }

        public int Rating { get; set; }
    }
}