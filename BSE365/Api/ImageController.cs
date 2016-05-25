using BSE365.Base.Repositories.Contracts;
using BSE365.Mappings;
using BSE365.Model.Entities;
using BSE365.Repository.Repositories;
using BSE365.ViewModels;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using BSE365.Mappings;
using System.Net;
using BSE365.Common.Constants;
using BSE365.Common.Helper;

namespace BSE365.Api
{
    [Authorize]
    [RoutePrefix("api/image")]
    public class ImageController : ApiController
    {
        private IRepositoryAsync<Image> _repo = null;
        IRepositoryAsync<PinTransactionHistory> _historyRepo;

        public ImageController(IRepositoryAsync<Image> repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Route("GetUserPicture")]
        public async Task<HttpResponseMessage> GetUserPicture(int id)
        {
            HttpResponseMessage response = null;
            if (id == 0)
            {
                var defaultPic = string.Format(@"{0}\Content\image\{1}", System.AppDomain.CurrentDomain.BaseDirectory, SystemAdmin.UserDefautPic);
                var content = System.IO.File.ReadAllBytes(defaultPic);
                response = new HttpResponseMessage { Content = new ByteArrayContent(content) };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(Utilities.GetImageContentType(defaultPic.Split('.').Last()));
                response.Content.Headers.ContentLength = content.Length;
            }
            else
            {
                var image = await _repo.FindAsync(id);
                if (image == null) throw new HttpResponseException(HttpStatusCode.NotFound);
                response = new HttpResponseMessage { Content = new ByteArrayContent(image.Content) };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(Utilities.GetImageContentType(image.Extension));
                response.Content.Headers.ContentLength = image.Content.Length;              
            }

            return response;
        }
    }
}

