using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
	public class SatisDetay:BaseEntity
	{
		public int SatisId { get; set; }
		public Satis Satis { get; set; } = null!;
		public int UrunId { get; set; }
		public Urun Urun { get; set; } = null!;
		public int Adet { get; set; }
		public int Fiyat { get; set; }
	}
}
