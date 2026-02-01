using EczaneApp.Data.Common;

namespace EczaneApp.Data
{
    public class Kategori:BaseEntity
    {
        public string KategoriAdi { get; set; } = null!;
        public ICollection<Urun>Urunler=new List<Urun>();
    }
}
