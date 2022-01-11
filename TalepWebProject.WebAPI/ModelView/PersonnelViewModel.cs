using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.API.ModelView
{
    public class PersonnelViewModel
    {
        [Key]
        public int PersonID { get; set; }
        public string PersonName { get; set; }
        public string PersonGender { get; set; }

       // [Required(ErrorMessage = "Zorunlu alan")]
       // [DataType(DataType.PhoneNumber)]
       // [MaxLength(13, ErrorMessage = "Telefon numarası 11 haneli olmalıdır"), MinLength(13, ErrorMessage = "Telefon numarası 11 haneli  olmalıdır")]
       //[RegularExpression("^(?!0+$)(\\+\\d{1,3}[?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Telefon numarası format dışı")]
        public string PersonPhone { get; set; }
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                ErrorMessage = "E-mail is not valid")]
        public string PersonEmail { get; set; }
        public string PersonPass { get; set; }
        public string PersonRolId { get; set; }
        public string PersonDate { get; set; }
        public string PersonPhoto { get; set; }
        public int PersonDurum { get; set; }
    }
}
