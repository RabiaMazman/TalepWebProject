using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.API.ModelView
{
    public class RequestViewModel
    {
        [Key]
        public int ReqID { get; set; }
        public string ReqPersId { get; set; }
        public string ReqCompId { get; set; }
        public string ReqDepId { get; set; }
        public string ReqRolId { get; set; }
        public string ReqProductCategoryId { get; set; }
        public string ReqProductNameId { get; set; }
        public int ReqProductNumber { get; set; }
        public string ReqProductBirimId { get; set; }
        public int ReqTahFiyat { get; set; }

        public string ReqOnayPersRolNameId { get; set; }
        public string ReqDate { get; set; }
        public string ReqOnay { get; set; }
        public string ReqOnayDate { get; set; }
    }
}
