namespace EczaneApp.Data
{
	public class UserEditVievModel
	{
		public int Id { get; set; } // Kullanıcıyı tanımlayan benzersiz ID
		public string İsim { get; set; } // Kullanıcının adı
		public string Soyisim { get; set; } // Kullanıcının adı

		public string Email { get; set; } // Kullanıcının e-posta adresi
		public long telefon { get; set; }	

		// Mevcut şifre ve yeni şifre (isteğe bağlı) alanları
		public string Sifre { get; set; }
		public string YeniSifre { get; set; }
	}
}
