using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
	public class Depo:BaseEntity
	{
		public string Depoİsmi { get; set; } = null!;
		public string Adres { get; set; } = null!;
		public ICollection<DepoStok> DepoStoklar { get; set; }=new List<DepoStok>();
		public ICollection<Stok> StokHareketleri { get; set; } = new List<Stok>();


	}
}
