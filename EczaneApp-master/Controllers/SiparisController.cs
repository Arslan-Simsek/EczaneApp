using EczaneApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EczaneApp.Controllers
{
	[Authorize]

	public class SiparisController : Controller
	{
		private readonly DataContext _context;


		public SiparisController(DataContext context)
		{
			_context = context;
		}

		// GET: Satis
		public async Task<IActionResult> Index()
		{
			var siparisler = await _context.Siparisler.OrderByDescending(x=>x.CreatedAt).ToListAsync();
			var tedarikciler = await _context.Tedarikciler.ToListAsync();
			var siparisDetaylari = await _context.SiparisDetaylari.ToListAsync();
			var urunler = await _context.Urunler.ToListAsync();

			var siparisViewModelList = siparisler.Select(siparis => new SiparisViewModel
			{
				SiparisId = siparis.Id,
				Tarih = siparis.CreatedAt,
				TedarikciAdi = tedarikciler.FirstOrDefault(t => t.Id == siparis.TedarikciId)?.TedarikciAdi,
				Status = siparis.status,
				ToplamFiyat=siparis.ToplamFİyat,
				SiparisDetaylari = siparisDetaylari
									.Where(sd => sd.SiparisId == siparis.Id)
									.Select(sd => new SiparisDetayViewModel
									{
										UrunId = sd.UrunId,
										Fiyat= sd.Fiyat,
										UrunAdi = urunler.FirstOrDefault(u => u.Id == sd.UrunId)?.Urunİsmi,
										Adet = sd.Adet
									}).ToList()
			}).ToList();

			return View(siparisViewModelList);
		}



		// GET: Satis/Details/5

		// GET: Satis/Create
		public IActionResult Create()
		{
			ViewBag.Tedarikcim = new SelectList(_context.Tedarikciler.ToList(), "Id", "TedarikciAdi");

			ViewBag.Urun = new SelectList(_context.Urunler.ToList(), "Id", "Urunİsmi");
			return View();
		}
		[HttpPost]


		public async Task<IActionResult> Create(Siparis siparis, [FromForm] List<SiparisDetay> SiparisDetaylari)
		{
			siparis.CreatedAt = DateTime.Now;
			siparis.SiparisDetaylari = SiparisDetaylari;
			siparis.status = 0; // Sipariş beklemede
			int toplamfiyat = 0;


			await _context.Siparisler.AddAsync(siparis);
			await _context.SaveChangesAsync();

			foreach (var item in SiparisDetaylari)
			{
				item.SiparisId = siparis.Id;
				var urun = await _context.Urunler.FindAsync(item.UrunId);
				if (urun == null)
				{
					ModelState.AddModelError(string.Empty, $"Ürün bulunamadı: {item.UrunId}");
					ViewBag.Urun = new SelectList(await _context.Urunler.ToListAsync(), "Id", "Urunİsmi");
					return View(siparis);
				}
				item.CreatedAt= DateTime.Now;	
				toplamfiyat += item.Fiyat * item.Adet;

			}
			siparis.ToplamFİyat = toplamfiyat;
			await _context.SiparisDetaylari.AddRangeAsync(SiparisDetaylari);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index", "Home");
		}
		//}
		[HttpPost]
		public async Task<IActionResult> UpdateStatus(int siparisId, byte newStatus)
		{
			var siparis = await _context.Siparisler.FindAsync(siparisId);
			siparis.UpdatedAt= DateTime.Now;
			if (siparis == null)
			{
				return NotFound();
			}

			siparis.status = newStatus;

			// Eğer durum "Geldi" ise, stok güncellemesini yap
			if (newStatus == 1)
			{
				var siparisDetaylari = await _context.SiparisDetaylari
					.Where(sd => sd.SiparisId == siparisId)
					.ToListAsync();

				foreach (var detay in siparisDetaylari)
				{
					var depoStok = await _context.DepoStoklari
						.FirstOrDefaultAsync(ds => ds.UrunId == detay.UrunId && ds.DepoID == 1);

					if (depoStok != null)
					{
						depoStok.Miktar += detay.Adet;
					}
					else
					{
						_context.DepoStoklari.Add(new DepoStok
						{
							UpdatedAt = DateTime.Now,
							DepoID = 1,
							UrunId = detay.UrunId,
							Miktar = detay.Adet
						});
					}

					// Stok hareketi ekle
					_context.Stoklar.Add(new Stok
					{
						
						UrunId = detay.UrunId,
						DepoId = 1,
						HareketTipi = true,
						Miktar = detay.Adet,
						CreatedAt = DateTime.Now
					});;
				}
			}

			await _context.SaveChangesAsync();
			return RedirectToAction("Index","Home");
		}
	}


}

