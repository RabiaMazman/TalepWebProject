using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.MVC.Models
{
    public class RoleView
    {

        [Key]
        public int RolID { get; set; }
        [Required(ErrorMessage = "Boş Değer Girilemez", AllowEmptyStrings = false)]

        public string RolName { get; set; }

    }
}
