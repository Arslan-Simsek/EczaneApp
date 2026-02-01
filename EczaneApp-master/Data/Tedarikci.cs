using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
    public class Tedarikci:BaseEntity
    {
        public string TedarikciAdi { get; set; } = null!;
        public string İrtibatKisisi { get; set; } = null!;
        public long TelefonNo { get; set; }
        public string Email { get; set; } = null!;
        public ICollection<Siparis> Siparis =new List<Siparis>();



    }
}
