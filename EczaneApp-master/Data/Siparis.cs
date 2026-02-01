using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
    public class Siparis:BaseEntity
    {
        public int TedarikciId { get; set; }
        public Tedarikci Tedarikci { get; set; } = null!;
        public ICollection<SiparisDetay>SiparisDetaylari=new List<SiparisDetay>();   
        public int ToplamFİyat {  get; set; }
        public byte status { get; set; }    
    }
}
