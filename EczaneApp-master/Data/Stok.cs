using EczaneApp.Data.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EczaneApp.Data
{
    public class Stok:BaseEntity
    {
        public int UrunId { get; set; }
        public Urun Urun { get; set; } = null!;
        public bool HareketTipi {  get; set; }
        public int Miktar {  get; set; }    
        public int DepoId { get; set; }
        public Depo Depo { get; set; } = null!;
    

    }
}
