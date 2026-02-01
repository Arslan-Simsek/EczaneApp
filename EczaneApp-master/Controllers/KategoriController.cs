using EczaneApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EczaneApp.Controllers
{
	[Authorize]

	public class KategoriController : Controller
	{
		private readonly DataContext _context;
		public KategoriController(DataContext context)
		{
			_context = context;
		}
		public  async Task<IActionResult> Index()
		{
			var kategoriler = await _context.Kategoriler.ToListAsync();
			return View(kategoriler);
		}
		public IActionResult Create()
		{
			return View();//create Viewini görüntülüyor.
		}
		[HttpPost]
		public async Task<IActionResult> Create(Kategori model)
		{
			model.CreatedAt = DateTime.Now;//formndan gelen bilgilere oluşturulma zamannını ekliyorum.
			_context.Kategoriler.Add(model);//veritabanı tabloma eklenecek olarak işaretliyorum
			await _context.SaveChangesAsync();//asenkron bir şekilde veritabanına kaydediyorum
			return RedirectToAction("Index", "Home");//anassayfa yönlendirmesi
		}
		public async Task<IActionResult> Edit(int id)
		{//ilgili kategorinin bilgilerinin forma gönderilmesi
			var kategori = await _context.Kategoriler.FirstOrDefaultAsync(x => x.Id == id);
			if (kategori == null) { return NotFound(); }
			return View(kategori);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Kategori model, int id)
		{
			if (id != model.Id)
			{//urldeki id ile kategorinin idsi aynımı ?
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{	//hata yoksa güncelle
					_context.Update(model);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{	//hata varsa not found ekranı döndür
					if (!_context.Kategoriler.Any(o => o.Id == model.Id))
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
		{	//url den id yakalayıp ilgili kategoriyi bulup silme
			var kategori = await _context.Kategoriler.FirstOrDefaultAsync(x => x.Id == id);
			if (kategori == null) { return NotFound(); }
			_context.Kategoriler.Remove(kategori);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index", "Kategori");
		}


	}
}

