using BSE365.Base.Repositories.Contracts;
using BSE365.Common.Constants;
using BSE365.Common.Helper;
using BSE365.Model.Entities;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace BSE365.Api
{
    [Authorize]
    [RoutePrefix("api/image")]
    public class ImageController : ApiController
    {
        private IRepositoryAsync<Image> _repo = null;

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
                var defaultPic = string.Format(@"{0}\Content\images\{1}", System.AppDomain.CurrentDomain.BaseDirectory,
                    SystemAdmin.UserDefautPic);
                var content = System.IO.File.ReadAllBytes(defaultPic);
                response = new HttpResponseMessage {Content = new ByteArrayContent(content)};
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(Utilities.GetImageContentType(defaultPic.Split('.').Last()));
                response.Content.Headers.ContentLength = content.Length;
            }
            else
            {
                var image = await _repo.FindAsync(id);
                if (image == null) throw new HttpResponseException(HttpStatusCode.NotFound);
                response = new HttpResponseMessage {Content = new ByteArrayContent(image.Content)};
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(Utilities.GetImageContentType(image.Extension));
                response.Content.Headers.ContentLength = image.Content.Length;
            }

            return response;
        }
    }
}