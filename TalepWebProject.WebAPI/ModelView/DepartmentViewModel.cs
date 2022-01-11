using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.WebAPI.ModelView
{
    public class DepartmentViewModel
    {
        [Key]
        public int DepartID { get; set; }
        public string DepartName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }


      
    }
}