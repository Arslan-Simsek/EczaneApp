using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
    public class Musteri:BaseLongEntity
    {
		public string İsim { get; set; } = null!;
		public string Soyisim { get; set; } = null!;
		public string? Email { get; set; } = null!;
		public long Telefon { get; set; }
        public string? Adres { get; set; }
		public DateTime DogumTarihi { get; set; }
		public ICollection<Recete>Receteler=new List<Recete>();
		public ICollection<Satis> Satislar = new List<Satis>();

	}
}
