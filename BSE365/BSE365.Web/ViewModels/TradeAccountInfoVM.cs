namespace BSE365.ViewModels
{
    public class TradeAccountInfoVM
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }

        public int? AvatarId { get; set; }

        public string AvatarUrl
        {
            get
            {
                var result = ImageViewModel.ImageUrl + (AvatarId ?? 0);
                return result;
            }
        }

        public string Email { get; set; }
        public string ParentId { get; set; }
        public string PhoneNumber { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }

        public int Rating { get; set; }
        public int Level { get; set; }
        public string TreePath { get; set; }
    }
}