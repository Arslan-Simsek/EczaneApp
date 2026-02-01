using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
    public class SiparisDetay:BaseEntity
    {
        public int SiparisId { get; set; }
        public Siparis Siparis { get; set; } = null!;
        public int UrunId { get; set; }
        public Urun Urun { get; set; } = null!;
        public int Adet {  get; set; }  
        public int Fiyat { get; set; }

		public static explicit operator SiparisDetay(Siparis v)
		{
			throw new NotImplementedException();
		}
	}
}
