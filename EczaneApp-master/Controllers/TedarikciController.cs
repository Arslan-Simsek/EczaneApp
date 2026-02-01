using EczaneApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EczaneApp.Controllers
{
	[Authorize]

	public class TedarikciController : Controller
	{

		private readonly DataContext _context;
		public TedarikciController(DataContext context)
		{
			_context = context;
		}


		public async Task<IActionResult> Index()
		{
			var tedarikciler = await _context.Tedarikciler.ToListAsync();
			return View(tedarikciler);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(Tedarikci model)
		{
			model.CreatedAt = DateTime.Now;
			_context.Tedarikciler.Add(model);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Edit(int id)
		{
			var tedarikci = await _context.Tedarikciler.FirstOrDefaultAsync(x => x.Id == id);
			if (tedarikci == null) { return NotFound(); }
			return View(tedarikci);
		}



		[HttpPost]
		public async Task<IActionResult> Edit(Tedarikci model, int id)
		{
			if (id != model.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					model.UpdatedAt= DateTime.Now;	
					_context.Tedarikciler.Update(model);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!_context.Tedarikciler.Any(o => o.Id == model.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction("Index");
			}

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var tedarik = await _context.Tedarikciler.FirstOrDefaultAsync(x => x.Id == id);
			if (tedarik == null) { return NotFound(); }
			_context.Tedarikciler.Remove(tedarik);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index", "Kategori");
		}
	}
}
