using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.MVC.Models
{
    public class TalepView
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
        public string TalepEdenPersonelName { get; set; }
        public string TalepEdenPersonelCompId { get; set; }


        public string TalepEdenPersonelDepId { get; set; }

        public string TalepDurumu { get; set; }
        public int TalepOnayDurum { get; set; }

        public string TalepEdenPersonelRolId { get; set; }

        public string TalepDate { get; set; }
        public int TalepOnaylayanYetkiliPersonelId { get; set; }
        public string TalepOnaylayanYetkiliPersonelName { get; set; }
        public string TalepOnaylayanYetkiliPersonelEmail{ get; set; }


        public string TalepOnayDate { get; set; }
    }
}
