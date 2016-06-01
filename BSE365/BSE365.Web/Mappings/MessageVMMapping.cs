using BSE365.Model.Entities;
using BSE365.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.Mappings
{
    public static class MessageVMMapping
    {
        public static MessageViewModel ToViewModel(this Message model)
        {
            if (model == null) return null;
            var result = new MessageViewModel
            {
                Content = model.Content,
                FromUser = model.FromId,
                ToUser = model.ToId,
                Status = model.Status,
                SendTime = model.SendTime
            };

            return result;
        }

        public static Message ToModel(this MessageViewModel viewModel)
        {
            if (viewModel == null) return null;
            var result = new Message
            {
                Content = viewModel.Content,
                FromId = viewModel.FromUser,
                ToId = viewModel.ToUser,
                Status = viewModel.Status,
                SendTime = viewModel.SendTime
            };

            return result;
        }        
    }
}