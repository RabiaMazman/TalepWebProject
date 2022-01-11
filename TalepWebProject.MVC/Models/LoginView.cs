using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.MVC.Models
{
    public class LoginView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PersonID { get; set; }
        public string PersonName { get; set; }
        public int GenderID { get; set; }
        public string PersonPhone { get; set; }

        [NotMapped]
        public byte[] PersonPhotoByte { get; set; }
        public string PersonPhoto { get; set; }

        public string PersonEmail { get; set; }
        public string PersonPass { get; set; }
        public string PersonCompanyId { get; set; }
        public string PersonDepartmentId { get; set; }
        public string PersonRolId { get; set; }
        [DataType(DataType.Date)]
        public string PersonDate { get; set; }
        public int PersonDurum { get; set; }
    }
}
