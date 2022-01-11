using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.MVC.Models
{
    public class DepartmentsView
    {
        [Key]
        public int DepartID { get; set; }
        [Required(ErrorMessage = "Boş Değer Girilemez", AllowEmptyStrings = false)]

        public string DepartName { get; set; }
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

    }
}
