using EczaneApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataContext>(options => 
{//veritabaný baðlantýsýný yapyým
	var config = builder.Configuration;
	var connectionString = config.GetConnectionString("database");
	options.UseSqlite(connectionString);
	
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
	options.LoginPath = "/Kullanici/GirisYap";//cookie ile giriþ iþlemi
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();//kullanýcý iþlemlerini etkin hale getirdim

app.UseAuthorization();

app.MapControllerRoute(//default url yolu 
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();//projenin ayaða kalktýðý kýsýmdýr
