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
                ToId = viewModel.ToId,
                Amount = viewModel.Amount,
                Note = viewModel.Note,
                Code = viewModel.Code,
            };

            return result;
        }        
    }
}