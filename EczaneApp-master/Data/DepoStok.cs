using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
	public class DepoStok:BaseEntity
	{
		public int DepoID { get; set; }
		public Depo Depo { get; set; } = null!;
		public int UrunId { get; set; }
		public Urun Urun { get; set; } = null!;
		public int Miktar { get; set; }

    }
}
