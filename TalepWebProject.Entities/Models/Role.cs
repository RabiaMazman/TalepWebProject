using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalepWebProject.Entities.Models
{
    public class Role
    {
        [Key]
        public int RolID { get; set; }
        public string RolName { get; set; }

      //  public int RolDeprtId { get; set; }
    }
}
