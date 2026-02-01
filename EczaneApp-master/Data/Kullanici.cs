using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
    public class Kullanici:BaseEntity
    {
        public string Sifre { get; set; } = null!;
        public string İsim { get; set; } = null!;
        public string Soyisim { get; set; } = null!;
        public string Email { get; set; } = null!;
        public long Telefon { get; set; } 
    }
}
