using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.MVC.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PersonID { get; set; }
        [Required(ErrorMessage = "Boş Değer Girilemez", AllowEmptyStrings = false)]

        public string PersonName { get; set; }
        [Required(ErrorMessage = "Boş Değer Girilemez", AllowEmptyStrings = false)]

        [NotMapped]
        public byte[] PersonPhotoByte { get; set; }
        public string PersonPhoto { get; set; }

        public string PersonGender { get; set; }
        public string PersonPhone { get; set; }
        public string PersonEmail { get; set; }
        public string PersonPass { get; set; }
        public string PersonCompanyId { get; set; }
        public string PersonDepartmentId { get; set; }
        public string PersonRolId { get; set; }
        public string PersonDate { get; set; }
        public int PersonDurum { get; set; }
    }
}
