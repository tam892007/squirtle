using BSE365.Model.Entities;
using BSE365.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.Mappings
{
    public static class UserContextVMMapping
    {
        public static UserContextViewModel ToContextViewModel(this User model)
        {
            if (model == null || model.UserInfo == null) return null;
            var result = new UserContextViewModel
            {
                Id = model.Id,
                UserName = model.UserName,
                PinBalance = model.PinBalance,
                Avatar = new ImageViewModel {Id = model.UserInfo.AvatarId.HasValue ? model.UserInfo.AvatarId.Value : 0},
            };

            return result;
        }
    }
}