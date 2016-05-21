using BSE365.Common;
using BSE365.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.Mappings
{
    public static class ResultViewModelMapping
    {
        public static ResultViewModel<TViewModel> ToResultViewModel<TViewModel, TModel>(this BusinessResult<TModel> businessResult, Func<TModel, TViewModel> func)
        {
            var result = new ResultViewModel<TViewModel>();

            if (businessResult == null)
            {
                return result;
            }

            result.IsSuccessful = businessResult.IsSuccessful;
            result.Result = func(businessResult.Result);
            return result;
        }
    }
}