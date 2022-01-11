using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalepWebProject.Entities.Models
{
    public class Personnels
    {
        [Key]
        public int PersonID { get; set; }
        public string PersonName { get; set; }
        public string PersonGender { get; set; }
        public string PersonPhone { get; set; }
        public string PersonEmail { get; set; }
        public string PersonPass { get; set; }
        public int PersonRolId { get; set; }
        public string PersonDate { get; set; }
        public int PersonDurum { get; set; }

        [NotMapped]
        public byte[] PersonPhotoByte { get; set; }
        public string PersonPhoto { get; set; }
        public bool MustChangePass { get; set; }
        public string RememberPassCode { get; set; }


    }

    public enum PersonGender
    {
        Kadın,
        Erkek
    }


}
