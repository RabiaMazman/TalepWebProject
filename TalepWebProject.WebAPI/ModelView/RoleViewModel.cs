using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.WebAPI.ModelView
{
    public class RoleViewModel
    {
        [Key]
        public int RolID { get; set; }
        public string RolName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }


        //public string RolDeprtId { get; set; }
    }
}
