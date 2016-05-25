using BSE365.Base.Repositories.Contracts;
using BSE365.Common.Constants;
using BSE365.Model.Entities;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BSE365.Controllers
{
    public class ImageController : Controller
    {
        private IRepositoryAsync<Image> _repo;
        public ImageController(IRepositoryAsync<Image> repo)
        {
            this._repo = repo;
        }

        [HttpGet]
        [Authorize()]
        public async Task<FileContentResult> Get(int id)
        {
            var image = await this._repo.FindAsync(id);

            if (image != null)
            {
                var ext = image.Extension.ToLower();

                if (ext.EndsWith(".png"))
                {
                    return new FileContentResult(image.Content, "image/png");
                }

                if (ext.EndsWith(".jpg") || ext.EndsWith(".jpeg"))
                {
                    return new FileContentResult(image.Content, "image/jpeg");
                }

                return new FileContentResult(image.Content, "application/octet-stream");
            }

            Response.StatusCode = 404;

            return null;
        }

        [HttpGet]
        [Authorize()]
        public async Task<FileContentResult> GetUserPicture(int id)
        {
            if (id == 0)
            {
                return new FileContentResult(System.IO.File.ReadAllBytes(string.Format(@"{0}\Content\image\{1}"
                    , System.AppDomain.CurrentDomain.BaseDirectory, SystemAdmin.UserDefautPic)), "image/jpeg"); 
            }
            else
            {
                return await Get(id);
            }
        }
    }
}