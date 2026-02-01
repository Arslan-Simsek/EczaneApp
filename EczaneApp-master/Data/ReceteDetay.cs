using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
    public class ReceteDetay:BaseEntity
    {
        public int ReceteId { get; set; }
		public Recete Recete { get; set; } = null!;

		public int UrunId {  get; set; }
        public Urun Urun { get; set; }=null!;
        public int Adet {  get; set; }

    }
}
