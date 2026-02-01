using EczaneApp.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]

public class SatisController : Controller
{
	private readonly DataContext _context;


	public SatisController(DataContext context)
	{
		_context = context;
	}

	// GET: Satis
	
		public async Task<IActionResult> Index()
		{
			// Satislar listesini al
			var satislar = await _context.Satislar.OrderByDescending(x=>x.CreatedAt).ToListAsync();

			// Her bir Satis için SatisDetaylari verilerini al
			foreach (var satis in satislar)
			{
				satis.SatisDetaylari = await _context.SatisDetaylari
													 .Where(sd => sd.SatisId == satis.Id)
													 .ToListAsync();

				// SatisDetaylari içindeki Urun bilgilerini de al
				foreach (var detay in satis.SatisDetaylari)
				{
					detay.Urun = await _context.Urunler
											   .Where(u => u.Id == detay.UrunId)
											   .FirstOrDefaultAsync();
				}
			}

			return View(satislar);
		}

	

	// GET: Satis/Details/5
	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var satis = await _context.Satislar
								  .Include(s => s.SatisDetaylari)
									  .ThenInclude(sd => sd.Urun)
								  .FirstOrDefaultAsync(m => m.Id == id);
		if (satis == null)
		{
			return NotFound();
		}

		return View(satis);
	}

	// GET: Satis/Create
	//public IActionResult Create( [FromRoute]int? id)
	//{

	//	ViewBag.Urun = new SelectList(_context.Urunler.ToList(), "Id", "Urunİsmi");
	//	return View();
	//}
	public IActionResult Create()
	{
		var urunler = _context.Urunler.ToList();
		var urunSelectList = urunler.Select(u => new SelectListItem
		{
			Value = u.Id.ToString(),
			Text = $"{u.Urunİsmi} - {u.Fiyat} TL"
		}).ToList();

		ViewBag.Urun = urunSelectList;
		ViewBag.UrunFiyatlari = urunler.ToDictionary(u => u.Id, u => u.Fiyat);

		return View();
	}





	[HttpPost]
	public async Task<IActionResult> Create(Satis satis, [FromForm] List<SatisDetay> SatisDetaylari)
	{
		satis.CreatedAt = DateTime.Now;
		satis.SatisDetaylari = SatisDetaylari;

		// Toplam fiyatı hesaplamak için bir değişken ekliyoruz
		int toplamFiyat = 0;

		using (var transaction = await _context.Database.BeginTransactionAsync())
		{
			try
			{
				await _context.Satislar.AddAsync(satis);
				await _context.SaveChangesAsync();

				foreach (var satisDetay in satis.SatisDetaylari)
				{
					satisDetay.SatisId = satis.Id;
					satisDetay.CreatedAt=DateTime.Now;

					var urun = await _context.Urunler.FindAsync(satisDetay.UrunId);
					if (urun == null)
					{
						ModelState.AddModelError(string.Empty, $"Ürün bulunamadı: {satisDetay.UrunId}");
						ViewBag.Urun = new SelectList(await _context.Urunler.ToListAsync(), "Id", "Urunİsmi");
						await transaction.RollbackAsync();
						ViewBag.UrunFiyatlari = _context.Urunler.ToDictionary(u => u.Id, u => u.Fiyat);

						return View(satis);
					}

					satisDetay.Fiyat = urun.Fiyat;
					toplamFiyat += urun.Fiyat * satisDetay.Adet;

					var depoStok = await _context.DepoStoklari
												 .FirstOrDefaultAsync(ds => ds.UrunId == satisDetay.UrunId && ds.DepoID == 1);

					if (depoStok != null)
					{
						if (depoStok.Miktar >= satisDetay.Adet)
						{
							depoStok.Miktar -= satisDetay.Adet;
						}
						else
						{
							ModelState.AddModelError(string.Empty, $"Yeterli stok yok: {urun.Urunİsmi}");
							ViewBag.Urun = new SelectList(await _context.Urunler.ToListAsync(), "Id", "Urunİsmi");
							await transaction.RollbackAsync();
							ViewBag.UrunFiyatlari = _context.Urunler.ToDictionary(u => u.Id, u => u.Fiyat);

							return View(satis);
						}
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Stok bulunamadı: {urun.Urunİsmi}");
						ViewBag.Urun = new SelectList(await _context.Urunler.ToListAsync(), "Id", "Urunİsmi");
						ViewBag.UrunFiyatlari = _context.Urunler.ToDictionary(u => u.Id, u => u.Fiyat);

						await transaction.RollbackAsync();
						return View(satis);
					}
				}

				satis.ToplamFİyat = toplamFiyat;

				await _context.SatisDetaylari.AddRangeAsync(satis.SatisDetaylari);
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				ModelState.AddModelError(string.Empty, $"Bir hata oluştu: {ex.Message}");
				ViewBag.Urun = new SelectList(await _context.Urunler.ToListAsync(), "Id", "Urunİsmi");
				ViewBag.UrunFiyatlari = _context.Urunler.ToDictionary(u => u.Id, u => u.Fiyat);
				await transaction.RollbackAsync();
				return View(satis);

			}
		}

		return RedirectToAction("Index", "Home");
	}

	public async Task<IActionResult> GetReceteDetaylari(int receteId,string musteriTc)
	{

		var receteDetaylari = await _context.ReceteDetaylari
									   .Where(rd => rd.ReceteId == receteId&&rd.Recete.MusteriId==long.Parse(musteriTc))
									   .Select(rd => new
									   {
										   urunIsmi = rd.Urun.Urunİsmi, // Urun tablosundaki ilaç adını alın
										   adet = rd.Adet
									   }).ToListAsync();


		return Json(receteDetaylari);
	}
	public async Task<IActionResult> Edit(int id)

	{
		var satisdetay = await _context.SatisDetaylari.Where(x => x.SatisId == id).ToListAsync();
		var satis = await _context.Satislar.FirstOrDefaultAsync(x => x.Id == id);
		satis.SatisDetaylari = satisdetay;
		var urunum = await _context.Urunler.ToListAsync();
		if (satis == null)
		{
			return NotFound();
		}

		// Eğer ViewBag ile veri gönderiyorsanız, gerekli dataları ekleyin.
		// Örneğin, ürünlerin listesini gönderin, ama dropdown list vs. kullanmayacağız.
	

		ViewBag.Urun = urunum.Select(u => new SelectListItem
		{
			Value = u.Id.ToString(),
			Text = $"{u.Urunİsmi} - {u.Fiyat} TL"
		}).ToList();
		ViewBag.UrunFiyatlari = urunum.ToDictionary(u => u.Id, u => u.Fiyat);



		return View(satis);
	}




	//public async Task<IActionResult> Edit(int id)

	//{
	//	var satisdetay = await _context.SatisDetaylari.Where(x => x.SatisId == id).ToListAsync();
	//	var satis = await _context.Satislar.FirstOrDefaultAsync(x => x.Id == id);
	//	satis.SatisDetaylari = satisdetay;
	//	if (satis == null)
	//	{
	//		return NotFound();
	//	}

	//	// Eğer ViewBag ile veri gönderiyorsanız, gerekli dataları ekleyin.
	//	// Örneğin, ürünlerin listesini gönderin, ama dropdown list vs. kullanmayacağız.
	//	ViewBag.Urun = new SelectList(_context.Urunler.ToList(), "Id", "Urunİsmi");

	//	return View(satis);
	//}
	[HttpPost]
	public async Task<IActionResult> Edit(Satis satis, List<SatisDetay> SatisDetaylari)
	{
		int toplamfiyat = 0;
	

		// Mevcut satış ve detayları alın
		var existingsatisdetay = await _context.SatisDetaylari.Where(x => x.SatisId == satis.Id).ToListAsync();
		var existingSatis = await _context.Satislar.FirstOrDefaultAsync(x => x.Id == satis.Id);
		existingSatis.SatisDetaylari = existingsatisdetay;
		satis.SatisDetaylari = SatisDetaylari;
		existingSatis.UpdatedAt = DateTime.Now;
											  

			if (existingSatis == null)
			{
				return NotFound();
			}

			// Stok güncellemeleri için geçici bir liste
			var stockUpdates = new List<DepoStok>();
			var stockErrors = new List<string>();

		// Satış nesnesini güncelle

			// Eski detayları çıkar
			_context.SatisDetaylari.RemoveRange(existingSatis.SatisDetaylari);

			// Yeni detayları ekle ve stok güncellemeleri yap
			foreach (var detay in SatisDetaylari)
			{
			var urun2 = await _context.Urunler.FindAsync(detay.UrunId);
			detay.Fiyat = urun2.Fiyat;
				detay.SatisId = existingSatis.Id;
				_context.SatisDetaylari.Add(detay);
			toplamfiyat += detay.Fiyat * detay.Adet;

			var depoStok = await _context.DepoStoklari
											 .FirstOrDefaultAsync(ds => ds.UrunId == detay.UrunId && ds.DepoID == 1);

				if (depoStok != null)
				{
				if (depoStok.Miktar >= detay.Adet)
				{
					// Stok miktarını güncelle
					depoStok.Miktar -= detay.Adet;
					stockUpdates.Add(depoStok);
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Stok yetersiz: {detay.Urun.Urunİsmi}");

					ViewBag.Urun = _context.Urunler.Select(u => new SelectListItem
					{
						Value = u.Id.ToString(),
						Text = $"{u.Urunİsmi} - {u.Fiyat} TL"
					}).ToList();
					ViewBag.UrunFiyatlari = _context.Urunler.ToDictionary(u => u.Id, u => u.Fiyat);
					return View(satis);

					// Stok yetersizse hata mesajı ekle
				}
			}
			else
			{
				ModelState.AddModelError(string.Empty, $"Stok yetersiz: {detay.Urun.Urunİsmi}");

				ViewBag.Urun = _context.Urunler.Select(u => new SelectListItem
				{
					Value = u.Id.ToString(),
					Text = $"{u.Urunİsmi} - {u.Fiyat} TL"
				}).ToList();
				ViewBag.UrunFiyatlari = _context.Urunler.ToDictionary(u => u.Id, u => u.Fiyat);
				return View(satis);
			}
			}

			// Mevcut satışa göre eski stokları geri ekle
			foreach (var eskiDetay in existingSatis.SatisDetaylari)
			{
				var eskiDepoStok = await _context.DepoStoklari
												  .FirstOrDefaultAsync(ds => ds.UrunId == eskiDetay.UrunId && ds.DepoID == 1);

				if (eskiDepoStok != null)
				{
					// Eski miktarı geri ekle
					eskiDepoStok.Miktar += eskiDetay.Adet;
				}
			}
			existingSatis.ToplamFİyat=toplamfiyat;

			// Stok güncellemelerini veritabanına kaydet
			_context.DepoStoklari.UpdateRange(stockUpdates);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index","Home");
		}
		
	}









// POST: Satis/Create


