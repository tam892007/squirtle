using BSE365.Base.UnitOfWork.Contracts;
using System.Web.Http;

namespace BSE365.Api
{
    public class BaseController : ApiController
    {
        protected readonly IUnitOfWorkAsync UnitOfWorkAsync;

        public BaseController(
            IUnitOfWorkAsync unit)
        {
            UnitOfWorkAsync = unit;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnitOfWorkAsync.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}