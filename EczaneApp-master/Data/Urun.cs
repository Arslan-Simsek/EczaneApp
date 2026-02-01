using EczaneApp.Data.Common;
using System.Net;

namespace EczaneApp.Data
{
    public class Urun:BaseEntity
    {
        public string Urunİsmi { get; set; } = null!;
		public string? Aciklama { get; set; } 
        public int KategoriId { get; set; }
        public Kategori Kategori { get; set; } = null!;
        public int Fiyat {  get; set; }
        public DateTime SonKullanmaTarihi { get; set; }

        public ICollection<ReceteDetay> ReceteDetay =new List<ReceteDetay>();
        public ICollection<DepoStok>Depolar=new List<DepoStok>();
        public ICollection<Stok> Stok = new List<Stok>();
        public ICollection<SatisDetay>SatisDetays = new List<SatisDetay>();
		public ICollection<SiparisDetay> SiparisDetaylari = new List<SiparisDetay>();







	}
}
