using EczaneApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EczaneApp.Controllers
{
	public class DepoController : Controller
	{
		private readonly DataContext _context;
		public DepoController(DataContext context)
		{
			_context = context;
		}


		[Authorize]

		public async Task<IActionResult> Index()
		{
			var Depolar = await _context.Depolar.ToListAsync();
			return View(Depolar);
		}
		[Authorize]

		public IActionResult Create()
		{
			return View();	
		}
		[Authorize]

		[HttpPost]
		public async Task<IActionResult> Create(Depo model)
		{
			model.CreatedAt = DateTime.Now;
			_context.Depolar.Add(model);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index", "Home");
		}
		[Authorize]

		public async Task<IActionResult> Edit(int id)
		{
			var depo = await _context.Depolar.FirstOrDefaultAsync(x => x.Id == id);
			if (depo == null) { return NotFound(); }
			return View(depo);
		}
		[Authorize]

		[HttpPost]
		public async Task<IActionResult> Edit(Depo model, int id)
		{
			model.UpdatedAt=DateTime.Now;
			if (id != model.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Depolar.Update(model);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!_context.Depolar.Any(o => o.Id == model.Id))
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
		[Authorize]

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var depo = await _context.Depolar.FirstOrDefaultAsync(x => x.Id == id);
			if (depo == null) { return NotFound(); }
			_context.Depolar.Remove(depo);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index", "Kategori");
		}
	}
}
