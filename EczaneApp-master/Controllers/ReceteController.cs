using Microsoft.AspNetCore.Mvc;

namespace EczaneApp.Controllers
{
	public class ReceteController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
