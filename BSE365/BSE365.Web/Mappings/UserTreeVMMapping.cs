using BSE365.Model.Entities;
using BSE365.ViewModels;

namespace BSE365.Mappings
{
    public static class UserTreeVMMapping
    {
        public static UserTreeViewModel ToViewModel(this UserTree model)
        {
            if (model == null) return null;
            var result = new UserTreeViewModel
            {
                UserId = model.UserId,
                UserName = model.UserName,
                NumberOfChildren = model.NumberOfChildren,
                DisplayName = model.DisplayName,
            };

            return result;
        }
    }
}