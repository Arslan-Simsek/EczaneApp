using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Xml.Linq;




namespace EczaneApp.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}
		public DbSet<Kategori> Kategoriler => Set<Kategori>();
		public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
		public DbSet<Depo> Depolar => Set<Depo>();
		public DbSet<DepoStok> DepoStoklari => Set<DepoStok>();
		public DbSet<Doktor> Doktorlar => Set<Doktor>();
		public DbSet<Satis> Satislar => Set<Satis>();
		public DbSet<SatisDetay> SatisDetaylari => Set<SatisDetay>();
		public DbSet<Musteri> Musteriler => Set<Musteri>();
		public DbSet<Recete> Receteler => Set<Recete>();
		public DbSet<ReceteDetay> ReceteDetaylari => Set<ReceteDetay>();
		public DbSet<Siparis> Siparisler => Set<Siparis>();
		public DbSet<SiparisDetay> SiparisDetaylari => Set<SiparisDetay>();
		public DbSet<Stok> Stoklar => Set<Stok>();
		public DbSet<Tedarikci> Tedarikciler => Set<Tedarikci>();
		public DbSet<Urun> Urunler => Set<Urun>();

















	}
}


