using BSE365.Base.Repositories.Contracts;
using BSE365.Base.UnitOfWork.Contracts;
using BSE365.Model.Entities;
using System.Threading.Tasks;
using System.Web.Http;

namespace BSE365.Api
{
    [RoutePrefix("api/Config")]
    public class ConfigController : BaseController
    {
        private readonly IRepositoryAsync<Config> _repository;

        public ConfigController(
            IUnitOfWorkAsync unit,
            IRepositoryAsync<Config> repository)
            : base(unit)
        {
            _repository = repository;
        }

        [Route("GetAll")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var result = _repository.Query().Select();
            return Ok(result);
        }
    }
}