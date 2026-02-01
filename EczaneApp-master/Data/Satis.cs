using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
	public class Satis:BaseEntity
	{
		public DateTime Tarih { get; set; }
		public int ToplamFİyat { get; set; }

		public ICollection<SatisDetay> SatisDetaylari = new List<SatisDetay>();

	}
}

