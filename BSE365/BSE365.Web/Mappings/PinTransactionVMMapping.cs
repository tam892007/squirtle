using BSE365.Model.Entities;
using BSE365.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.Mappings
{
    public static class PinTransactionVMMapping
    {        
        public static PinTransaction ToModel(this PinTransactionViewModel viewModel)
        {
            if (viewModel == null) return null;
            var result = new PinTransaction
            {
                FromId = viewModel.FromId,
                FromName = viewModel.FromName,
                ToId = viewModel.ToId,
                Amount = viewModel.Amount,
                Note = viewModel.Note,
                Code = viewModel.Code,
            };

            return result;
        }

        public static PinTransactionHistoryViewModel ToViewModel(this PinTransactionHistory model)
        {
            if (model == null) return null;
            var result = new PinTransactionHistoryViewModel
            {
                FromId = model.FromId,
                FromName = model.FromName,
                ToId = model.ToId,
                Amount = model.Amount,
                Note = model.Note,
                Code = model.Code,
                CreatedDate = model.CreatedDate,
            };

            return result;
        }        
    }
}