using BSE365.Model.Entities;
using BSE365.ViewModels;

namespace BSE365.Mappings
{
    public static class PinBalanceVMMapping
    {
        public static PinBalanceViewModel ToPinBalanceViewModel(this User model)
        {
            if (model == null || model.UserInfo == null) return null;
            var result = new PinBalanceViewModel
            {
                UserId = model.Id,
                UserName = model.UserName,
                PinBalance = model.PinBalance,
            };

            return result;
        }     
    }
}