using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
    public class Recete:BaseEntity
    {
        public long MusteriId { get; set; }
        public Musteri Musteri { get; set; } = null!;
        public int DoktorId { get; set; }
        public Doktor Doktor { get; set; } = null!;

        public DateTime ReceteTarihi { get; set; }
        public string? Notlar {  get; set; }
        public ICollection<ReceteDetay> ReceteDetaylari =new List<ReceteDetay>();   
    }
}
