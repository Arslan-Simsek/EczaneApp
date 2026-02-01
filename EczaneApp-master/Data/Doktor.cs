using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
	public class Doktor:BaseEntity
	{
		public string Ad { get; set; }
		public string Soyad { get; set; }
		public string Uzmanlik { get; set; } = null!;
		public ICollection<Recete> Receteler=new List<Recete>();
		
	}
}
