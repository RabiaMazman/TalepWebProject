using Microsoft.EntityFrameworkCore;

namespace TalepWebProject.Entities.Models
{
    [Keyless]
    public class Personel_Company_Deparment
    {
        public int PersonelId { get; set; }
        public int DepartId { get; set; }
        public int CompanyId { get; set; }
    }
}
