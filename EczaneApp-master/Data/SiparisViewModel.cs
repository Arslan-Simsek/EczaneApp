namespace EczaneApp.Data
{
	public class SiparisViewModel
	{
		public int SiparisId { get; set; }
		public DateTime Tarih { get; set; }
		public string TedarikciAdi { get; set; }
		public byte Status { get; set; }
		public List<SiparisDetayViewModel> SiparisDetaylari { get; set; }
		public int ToplamFiyat {  get; set; }
	}

	public class SiparisDetayViewModel
	{
		public int UrunId { get; set; }
		public string UrunAdi { get; set; }
		public int Adet { get; set; }
		public int Fiyat { get; set; }
	}
}
