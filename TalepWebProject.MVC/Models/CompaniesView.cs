using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.MVC.Models
{
    public class CompaniesView
    {
        [Key]
        public int CompanyID { get; set; }
        [Required(ErrorMessage ="Boş Değer Girilemez",AllowEmptyStrings =false)]
        public string CompanyName { get; set; }
    }
}
