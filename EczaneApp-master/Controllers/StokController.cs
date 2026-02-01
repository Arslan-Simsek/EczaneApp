using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EczaneApp.Models;
using System.Linq;
using System.Threading.Tasks;
using EczaneApp.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize]

public class StokController : Controller
{
	private readonly DataContext _context;

	public StokController(DataContext context)
	{
		_context = context;
	}

	// GET: StokHareket
	public async Task<IActionResult> Index()
	{
		var stokHareketleri = _context.Stoklar
									  .Include(sh => sh.Depo)
									  .Include(sh => sh.Urun);
		return View(await stokHareketleri.ToListAsync());
	}



	// GET: StokHareket/Create
	public IActionResult Create()
	{

		ViewBag.Depo = new SelectList(_context.Depolar, "Id", "Depoİsmi");
		ViewBag.Urun = new SelectList(_context.Urunler, "Id", "Urunİsmi");
		return View();
	}

	// POST: StokHareket/Create
	[HttpPost]

	public async Task<IActionResult> Create( Stok stokHareket)
	{
		
		
			// DepoStok tablosundaki ilgili kaydı bul
			var depoStok = await _context.DepoStoklari
										 .FirstOrDefaultAsync(ds => ds.DepoID == stokHareket.DepoId && ds.UrunId == stokHareket.UrunId);

			if (depoStok == null)
			{
				// Eğer böyle bir kayıt yoksa yeni bir kayıt oluştur
				depoStok = new DepoStok
				{	CreatedAt=DateTime.Now,
					DepoID = stokHareket.DepoId,
					UrunId = stokHareket.UrunId,
					Miktar = 0
				};
				_context.DepoStoklari.Add(depoStok);
			}

			// Stok hareketine göre miktarı güncelle
			if (stokHareket.HareketTipi == true)
			{
				depoStok.Miktar += stokHareket.Miktar;
			}
			else if (stokHareket.HareketTipi == false)
			{
				if (depoStok.Miktar >= stokHareket.Miktar)
				{
					depoStok.Miktar -= stokHareket.Miktar;
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Yeterli stok yok.");
					ViewBag.Depo = new SelectList(_context.Depolar.ToList(), "DepoID", "DepoAdi", stokHareket.DepoId);
					ViewBag.Urun = new SelectList(_context.Urunler.ToList(), "Id", "Urunİsmi", stokHareket.UrunId);
					return View(stokHareket);
				}
			}
			stokHareket.CreatedAt=DateTime.Now;
			_context.Stoklar.Add(stokHareket);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index","Home");
		
		
	}
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var stokHareket = await _context.Stoklar.FindAsync(id);
		if (stokHareket == null)
		{
			return NotFound();
		}
		ViewBag.Depo = new SelectList(_context.Depolar, "Id", "Depoİsmi");
		ViewBag.Urun = new SelectList(_context.Urunler, "Id", "Urunİsmi");
		return View(stokHareket);
	}
	[HttpPost]
	public async Task<IActionResult> Edit(Stok stokHareket,int id)
	{
		if (id != stokHareket.Id)
		{
			return NotFound();
		}

		
			try
			{
				var originalStokHareket = await _context.Stoklar.AsNoTracking().FirstOrDefaultAsync(sh => sh.Id == id);
				if (originalStokHareket != null)
				{
					var originalDepoStok = await _context.DepoStoklari
														 .FirstOrDefaultAsync(ds => ds.DepoID == originalStokHareket.DepoId && ds.UrunId == originalStokHareket.UrunId);
					if (originalDepoStok != null)
					{
						if (originalStokHareket.HareketTipi == true)
						{
							originalDepoStok.Miktar -= originalStokHareket.Miktar;
						}
						else if (originalStokHareket.HareketTipi == false)
						{
							originalDepoStok.Miktar += originalStokHareket.Miktar;
						}
					}

					var newDepoStok = await _context.DepoStoklari
													.FirstOrDefaultAsync(ds => ds.DepoID == stokHareket.DepoId && ds.UrunId == stokHareket.UrunId);
					if (newDepoStok == null)
					{
						newDepoStok = new DepoStok
						{
							CreatedAt= DateTime.Now,
							DepoID = stokHareket.DepoId,
							UrunId = stokHareket.UrunId,
							Miktar = 0
						};
						_context.DepoStoklari.Add(newDepoStok);
					}

					if (stokHareket.HareketTipi == true&&originalDepoStok!=null)
					{
						newDepoStok.Miktar += stokHareket.Miktar;
					}
					else if (stokHareket.HareketTipi == false&&originalDepoStok!=null)
					{
						if (originalDepoStok.Miktar >= stokHareket.Miktar)
						{
							newDepoStok.Miktar -= stokHareket.Miktar;
						}
						else
						{
							ModelState.AddModelError(string.Empty, "Yeterli stok yok.");
							ViewBag.Depo = new SelectList(_context.Depolar, "Id", "Depoİsmi");
							ViewBag.Urun = new SelectList(_context.Urunler, "Id", "Urunİsmi");
							return View(stokHareket);
						}
					}

					_context.Stoklar.Update(stokHareket);
					await _context.SaveChangesAsync();
				}
			}
			catch (DbUpdateConcurrencyException)
			{
			
				
				
					throw;
				
			}
			return RedirectToAction(nameof(Index));
		}
	public IActionResult Transfer()
	{
		ViewBag.DepoKaynak = new SelectList(_context.Depolar, "Id", "Depoİsmi");
		ViewBag.DepoHedef = new SelectList(_context.Depolar, "Id", "Depoİsmi");
		ViewBag.Urun = new SelectList(_context.Urunler, "Id", "Urunİsmi");
		return View();
	}
	[HttpPost]
	public async Task<IActionResult> Transfer(UrunTransferiViewModel model)
	{
		
			// Kaynak depo stok bilgilerini al
			var kaynakDepoStok = await _context.DepoStoklari
				.FirstOrDefaultAsync(ds => ds.DepoID == model.KaynakDepoId && ds.UrunId == model.UrunId);

			// Hedef depo stok bilgilerini al
			var hedefDepoStok = await _context.DepoStoklari
				.FirstOrDefaultAsync(ds => ds.DepoID == model.HedefDepoId && ds.UrunId == model.UrunId);

			if (kaynakDepoStok == null || kaynakDepoStok.Miktar < model.Miktar)
			{
				ModelState.AddModelError("", "Kaynak depoda yeterli stok bulunamadı.");
				ViewBag.DepoKaynak = new SelectList(_context.Depolar, "Id", "Depoİsmi", model.KaynakDepoId);
				ViewBag.DepoHedef = new SelectList(_context.Depolar, "Id", "Depoİsmi", model.HedefDepoId);
				ViewBag.Urun = new SelectList(_context.Urunler, "Id", "Urunİsmi", model.UrunId);
				return View(model);
			}

		
			kaynakDepoStok.Miktar -= model.Miktar;
		_context.DepoStoklari.Update(kaynakDepoStok);

		// 2. Adım: Hedef depoya ürün miktarını ekle
		if (hedefDepoStok == null)
		{
			hedefDepoStok = new DepoStok
			{
				DepoID = model.HedefDepoId,
				UrunId = model.UrunId,
				Miktar = model.Miktar,
				CreatedAt = DateTime.Now
			};
			_context.DepoStoklari.Add(hedefDepoStok);
		}
		else
		{
			hedefDepoStok.Miktar += model.Miktar;
			hedefDepoStok.UpdatedAt = DateTime.Now;
			_context.DepoStoklari.Update(hedefDepoStok);
		}

		
	   var stokHareket = new Stok
			{
				DepoId = model.KaynakDepoId,
				UrunId = model.UrunId,
				Miktar = model.Miktar,
				HareketTipi = false, // Kaynaktan çıkarma
				CreatedAt = model.Tarih
			};
			_context.Stoklar.Add(stokHareket);

			stokHareket = new Stok
			{
				DepoId = model.HedefDepoId,
				UrunId = model.UrunId,
				Miktar = model.Miktar,
				HareketTipi = true, // Hedefe ekleme
				CreatedAt = model.Tarih
			};
			_context.Stoklar.Add(stokHareket);

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}


	


}




