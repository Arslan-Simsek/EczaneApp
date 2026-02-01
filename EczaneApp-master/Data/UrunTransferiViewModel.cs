namespace EczaneApp.Data
{
	public class UrunTransferiViewModel
	{
			public int UrunId { get; set; }
			public int KaynakDepoId { get; set; }
			public int HedefDepoId { get; set; }
			public int Miktar { get; set; }
			public DateTime Tarih { get; set; }

			public virtual Urun Urun { get; set; }
			public virtual Depo KaynakDepo { get; set; }
			public virtual Depo HedefDepo { get; set; }
		

	}
}
