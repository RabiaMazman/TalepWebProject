using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalepWebProject.Entities.Models
{
    public class Departments
    {
        [Key]
        public int DepartID { get; set; }
        public string DepartName { get; set; }
        public int CompanyId { get; set; }
        //public int DepartCompId { get; set; }
    }
}
