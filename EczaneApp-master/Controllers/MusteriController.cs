using EczaneApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EczaneApp.Controllers
{
	public class MusteriController : Controller
	{
		private readonly DataContext _context;
		public MusteriController(DataContext context)
		{
			_context = context;
		}


		public async Task<IActionResult> Index()
		{
			var Hasta = await _context.Musteriler.Include(x=>x.Receteler).Include(x=>x.Satislar).ToListAsync();
			return View(Hasta);
		}
		
	}
}
