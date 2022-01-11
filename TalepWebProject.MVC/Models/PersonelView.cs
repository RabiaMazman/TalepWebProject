using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.MVC.Models
{
    public class PersonelView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PersonID { get; set; }
        [Required(ErrorMessage = "Boş Değer Girilemez", AllowEmptyStrings = false)]

        public string PersonName { get; set; }
        [Required(ErrorMessage = "Boş Değer Girilemez", AllowEmptyStrings = false)]

        public string PersonGender { get; set; }

       
        public string PersonPhone { get; set; }
        public string PersonEmail { get; set; }
        public string PersonPass { get; set; }
        public string PersonRolId { get; set; }
        public string PersonDate { get; set; }
        public IFormFile PersonPhotoFile { get; set; }
        public byte[] PersonPhotoByte { get; set; }
        public string PersonPhoto { get; set; }
        public int PersonDurum { get; set; }
        public bool MustChangePass { get; set; }
        public string RememberPassCode { get; set; }
    }


    public class PersonelViewForLogin
    {
        public int PersonID { get; set; }

        public string PersonName { get; set; }

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

    public enum PersonGender
    {
        Kadın,
        Erkek
    }
    //public enum PersonDurum
    //{
    //    Aktif=1,
    //    Pasif=0
    //}

}
