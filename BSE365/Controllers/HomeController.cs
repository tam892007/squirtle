using System.Reflection;
using System.Web.Mvc;

namespace BSE365.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            this.ViewBag.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();            
            return View();
        }
    }
}