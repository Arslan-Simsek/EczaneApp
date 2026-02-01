using EczaneApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EczaneApp.Controllers
{
	[Authorize]

	public class UrunController : Controller
	{
		private readonly DataContext _context;
		public UrunController(DataContext context)
		{
			_context = context;
		}


		public IActionResult Index(string aramaTerimi = "", int sayfaNumarasi = 1)
		{
			int sayfaBoyutu = 10; // Her sayfada gösterilecek ürün sayısı
			int sayfaBasi = (sayfaNumarasi - 1) * sayfaBoyutu;

			var urunlerQuery = _context.Urunler.AsQueryable();

			if (!string.IsNullOrEmpty(aramaTerimi))
			{
				urunlerQuery = urunlerQuery.Where(u => u.Urunİsmi.Contains(aramaTerimi));
			}

			var urunler = urunlerQuery.Include(z=>z.Kategori)
				.Skip(sayfaBasi)
				.Take(sayfaBoyutu)
				.ToList();

			int toplamUrunSayisi = urunlerQuery.Count();
			int toplamSayfaSayisi = (int)Math.Ceiling((double)toplamUrunSayisi / sayfaBoyutu);

			ViewBag.ToplamSayfaSayisi = toplamSayfaSayisi;
			ViewBag.SayfaNumarasi = sayfaNumarasi;
			ViewBag.AramaTerimi = aramaTerimi; // Arama terimini ViewBag'e geçirin

			return View(urunler);
		}

		public IActionResult Create()
		{		//viewbag ile dinamik şekilde kategori selectbox bilgilerini gönderiylorum
				ViewBag.Kategorim = new SelectList(_context.Kategoriler, "Id", "KategoriAdi");
				return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(Urun model)
		{
			model.CreatedAt = DateTime.Now;
			_context.Urunler.Add(model);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index", "Home");
		}
		public async Task<IActionResult> Edit(int id)
		{
			ViewBag.Kategorim = new SelectList(_context.Kategoriler, "Id", "KategoriAdi");

			var urun = await _context.Urunler.FirstOrDefaultAsync(x => x.Id == id);
			if (urun == null) { return NotFound(); }
			return View(urun);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Urun model, int id)
		{
			if (id != model.Id)
			{
				return NotFound();
			}
					_context.Urunler.Update(model);
					await _context.SaveChangesAsync();

			return RedirectToAction("Index", "Home");

		}
		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var urun = await _context.Urunler.FirstOrDefaultAsync(x => x.Id == id);
			if (urun == null) { return NotFound(); }
			_context.Urunler.Remove(urun);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index", "Kategori");
		}
	}
}
