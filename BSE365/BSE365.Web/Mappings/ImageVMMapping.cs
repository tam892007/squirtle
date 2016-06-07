using BSE365.Model.Entities;
using BSE365.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE365.Mappings
{
    public static class ImageVMMapping
    {
        public static ImageViewModel ToViewModel(this Image model)
        {
            if (model == null) return null;
            var result = new ImageViewModel
            {
                Id = model.Id,
                Extension = model.Extension,
                Content = model.Content,
            };

            return result;
        }

        public static Image ToModel(this ImageViewModel viewModel)
        {
            if (viewModel == null) return null;
            var result = new Image
            {
                Id = viewModel.Id,
                Extension = viewModel.Extension,
                Content = viewModel.Content,
            };

            return result;
        }
    }
}