using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalepWebProject.Entities.Models
{
    public class Talepler
    {
        [Key]
        public int TalepID { get; set; }
        public string TalepName { get; set; }
        public string TalepAcıklaması { get; set; }
        public string TalepBirim { get; set; }
        public int TalepMiktari { get; set; }
        public int TalepTahminiFiyat { get; set; }

        public string TalepFiyatBirim { get; set; }
        public int TalepEdenPersonelId { get; set; }
        public int TalepEdenPersonelCompId { get; set; }
        public int TalepEdenPersonelDepId { get; set; }
        public int TalepEdenPersonelRolId { get; set; }

        public string TalepDate { get; set; }
        public int TalepOnaylayanYetkiliPersonelId { get; set; }
        public string TalepDurumu { get; set; }

        public int TalepOnayDurum { get; set; }
        
        public string TalepOnayDate { get; set; }  
   
    }
}
