using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalepWebProject.Entities.Models
{
    public class Companies
    {
        [Key]
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
    }
}
