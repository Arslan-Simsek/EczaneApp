using EczaneApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EczaneApp.Models;
using Microsoft.AspNetCore.Identity;

namespace EczaneApp.Controllers
{
	public class KullaniciController : Controller
	{
		private readonly DataContext _context;
		public KullaniciController(DataContext context)
		{
			_context = context;
		}
		public IActionResult Kaydol()
		{
			return View();
		}
		[HttpPost]
		
		public async Task<IActionResult> Kaydol(Kullanici model)
		{
			var user = await _context.Kullanicilar.FirstOrDefaultAsync(x => x.Email == model.Email);
			if (user == null)
			{
				// Şifreyi hashleyin
				var passwordHasher = new PasswordHasher<Kullanici>();
				model.Sifre = passwordHasher.HashPassword(model, model.Sifre);

				_context.Kullanicilar.Add(model);
				await _context.SaveChangesAsync();
				return RedirectToAction("GirisYap");
			}
			else
			{
				ModelState.AddModelError("", "Username ya da Email kullanımda.");
			}

			return View(model);
		}

		public IActionResult GirisYap()
		{
			if (User.Identity!.IsAuthenticated)
			{
				return RedirectToAction("Index", "Kategori");
			}
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> GirisYap(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _context.Kullanicilar
										 .FirstOrDefaultAsync(x => x.Email == model.Email);
				if (user != null)
				{
					var passwordHasher = new PasswordHasher<Kullanici>();
					var result = passwordHasher.VerifyHashedPassword(user, user.Sifre, model.Sifre);

					if (result == PasswordVerificationResult.Success)
					{
						var userClaims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.GivenName, user.İsim ?? "")
				};

						var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

						var authProperties = new AuthenticationProperties
						{
							IsPersistent = true
						};

						await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
						await HttpContext.SignInAsync(
							CookieAuthenticationDefaults.AuthenticationScheme,
							new ClaimsPrincipal(claimsIdentity),
							authProperties);

						return RedirectToAction("Index", "Kategori");
					}
					else
					{
						ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış");
					}
				}
				else
				{
					ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış");
				}
			}

			return View(model);
		}
		public async Task<IActionResult> CikisYap()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("GirisYap");
		}
		public async Task<IActionResult> Edit(int id)
		{
			var user = await _context.Kullanicilar.FindAsync(id);
			if (user == null)
			{
				return NotFound();
			}


			var model = new UserEditVievModel
			{
				Id = id,
				Email = user.Email,
				İsim = user.İsim,
				Soyisim = user.Soyisim,
				telefon = user.Telefon,
			};

			return View(model);
		}

		// Kullanıcı bilgilerini güncellemek için
		[HttpPost]
		public async Task<IActionResult> Edit(UserEditVievModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _context.Kullanicilar.FindAsync(model.Id);
				if (user == null)
				{
					return NotFound();
				}

				user.İsim = model.İsim;
				user.Email = model.Email;
				user.Telefon = model.telefon;
				user.Soyisim = model.Soyisim;

				var passwordHasher = new PasswordHasher<Kullanici>();

				if (!string.IsNullOrEmpty(model.YeniSifre))
				{
					var result = passwordHasher.VerifyHashedPassword(user, user.Sifre, model.Sifre);
					if (result == PasswordVerificationResult.Success)
					{
						if (model.YeniSifre == user.Sifre)
						{
							ModelState.AddModelError("", "Yeni Şifre Eski ile aynı olamaz");
							return View(model);
						}
						user.Sifre = passwordHasher.HashPassword(user, model.YeniSifre);
					}
					else
					{
						ModelState.AddModelError("", "Şifrenizi yanlış girdiniz");
						return View(model);
					}
				}
				else
				{
					ModelState.AddModelError("", "Yeni şifre boş olamaz");
					return View(model);
				}

				_context.Update(user);
				await _context.SaveChangesAsync();
				return RedirectToAction("GirisYap");
			}

			return View(model);
		}


	}
}
