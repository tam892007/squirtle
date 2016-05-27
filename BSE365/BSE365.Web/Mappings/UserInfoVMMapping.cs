using BSE365.Model.Entities;
using BSE365.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.Mappings
{
    public static class UserInfoVMMapping
    {
        public static UserInfoViewModel ToViewModel(this User model)
        {
            if (model == null || model.UserInfo == null) return null;
            var result = new UserInfoViewModel
            {
                Id = model.Id,
                UserName = model.UserName,
                DisplayName = model.UserInfo.DisplayName,
                Email = model.UserInfo.Email,
                ParentId = model.UserInfo.ParentId,
                PhoneNumber = model.UserInfo.PhoneNumber,
                BankNumber = model.UserInfo.BankNumber,
                BankName = model.UserInfo.BankName,
                BankBranch = model.UserInfo.BankBranch,
                Avatar = new ImageViewModel { Id = model.UserInfo.AvatarId.HasValue ? model.UserInfo.AvatarId.Value : 0 }
            };

            return result;
        }

        public static UserInfo ToModel(this UserInfoViewModel viewModel)
        {
            if (viewModel == null) return null;
            var result = new UserInfo
            {
                DisplayName = viewModel.DisplayName,
                Email = viewModel.Email,
                ParentId = viewModel.ParentId,
                PhoneNumber = viewModel.PhoneNumber,
                BankNumber = viewModel.BankNumber,
                BankName = viewModel.BankName,
                BankBranch = viewModel.BankBranch,
                Avatar = viewModel.Avatar.ToModel()
            };

            return result;
        }        
    }
}