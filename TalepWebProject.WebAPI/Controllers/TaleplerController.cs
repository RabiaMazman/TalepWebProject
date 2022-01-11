using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalepWebProject.Entities;
using TalepWebProject.Entities.Models;
using TalepWebProject.WebAPI.Bussines;
using TalepWebProject.WebAPI.ModelView;


namespace TalepWebProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaleplerController : ControllerBase
    {

        private readonly TalepDbContext _context;

        public TaleplerController(TalepDbContext context)
        {
            _context = context;
        }
        // GET: api/<TaleplerController>
        [HttpGet]
        public IEnumerable<TalepViewModel> Get(string startDate, string endDate)
        {
            List<Talepler> List = null;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                List = _context.Talepler.ToList()
                    .Where(x=> Convert.ToDateTime(x.TalepDate)>=Convert.ToDateTime(startDate + " 00:00:00") 
                    && Convert.ToDateTime(x.TalepDate)<= Convert.ToDateTime(endDate + " 23:59:29"))
                    .OrderByDescending(x => x.TalepID).ToList();
            else
                List = _context.Talepler.OrderByDescending(x => x.TalepID).ToList();

            var talepler = new Talepler();
            var TalepList = new List<TalepViewModel>();

            foreach (var item in List)
            {
                var talep = new TalepViewModel();
                talep.TalepID = item.TalepID;
                talep.TalepName = item.TalepName;
                talep.TalepAcıklaması = item.TalepAcıklaması;
                talep.TalepBirim = item.TalepBirim;
                talep.TalepMiktari = item.TalepMiktari;
                talep.TalepTahminiFiyat = item.TalepTahminiFiyat;
                talep.TalepFiyatBirim = item.TalepFiyatBirim;
                talep.TalepEdenPersonelId = (GetTalepEdenPersonelById(item.TalepEdenPersonelId)) == null ? 0 : (GetTalepEdenPersonelById(item.TalepEdenPersonelId)).PersonID;
                talep.TalepEdenPersonelName = (GetTalepEdenPersonelById(item.TalepEdenPersonelId)) == null ? "" :(GetTalepEdenPersonelById(item.TalepEdenPersonelId)).PersonName ;
                talep.TalepEdenPersonelCompId = GetTalepEdenPersonelCompIdById(item.TalepEdenPersonelCompId) == null ? null : GetTalepEdenPersonelCompIdById(item.TalepEdenPersonelCompId);
                talep.TalepEdenPersonelDepId = GetTalepEdenPersonelDepIdById(item.TalepEdenPersonelDepId) == null ? null : GetTalepEdenPersonelDepIdById(item.TalepEdenPersonelDepId);
                talep.TalepEdenPersonelRolId = GetTalepEdenPersonelRolIdById(item.TalepEdenPersonelRolId) == null ? null : GetTalepEdenPersonelRolIdById(item.TalepEdenPersonelRolId);

                talep.TalepDurumu = item.TalepDurumu;
                talep.TalepOnayDurum = item.TalepOnayDurum;
                talep.TalepDate = item.TalepDate;
                talep.TalepOnaylayanYetkiliPersonelId = item.TalepOnaylayanYetkiliPersonelId;
                talep.TalepOnaylayanYetkiliPersonelName = GetTalepOnaylayanYetkiliPersonelById(item.TalepOnaylayanYetkiliPersonelId) == null ? null : GetTalepOnaylayanYetkiliPersonelById(item.TalepOnaylayanYetkiliPersonelId);
               // talep.TalepOnaylayanYetkiliPersonelEmail = GetTalepOnaylayanYetkiliPersonelById(item.TalepOnaylayanYetkiliPersonelId) == null ? null : GetTalepOnaylayanYetkiliPersonelById(item.TalepOnaylayanYetkiliPersonelId);

                talep.TalepOnayDate = item.TalepOnayDate;

                TalepList.Add(talep);


               
            }
            return TalepList;
        }

        [HttpGet("GetTalepler/{personId}")]
        public List<Talepler> GetTalepler(int personID)
        {
            var result = _context.Talepler.Where(x => x.TalepOnaylayanYetkiliPersonelId == personID).ToList();
            return result;
        }

        [HttpPost("TalepDurumDegistir/{talepId}/{personId}")]
        public void TalepDurumDegistir(int talepId, int personId, [FromBody] int durum)
        {
            var entity = _context.Talepler.First(x => x.TalepID == talepId );
            entity.TalepOnaylayanYetkiliPersonelId = personId;
            entity.TalepOnayDurum = durum;
            entity.TalepDurumu = durum == 1 ? "Onaylandı" : "Reddedildi";
            entity.TalepOnayDate = DateTime.Now.ToString();
            _context.SaveChanges();



            var talepAcanMail = _context.Personnels.FirstOrDefault(x => x.PersonID == entity.TalepEdenPersonelId).PersonEmail;
            var onayName = _context.Personnels.FirstOrDefault(x => x.PersonID == entity.TalepOnaylayanYetkiliPersonelId).PersonName;

            MailService.Send(talepAcanMail, "Talep " + (durum == 1 ? "Onaylandı" : "Reddedildi"),
                $@"<p>Oluşturmuş olduğunuz talep {onayName} tarafından {(durum == 1 ? "onaylanmıştır" : "reddedilmiştir")}.</p>
                   <p>Taleb Adı:{entity.TalepName}</p>
                   <p>Talep Acıklaması:{entity.TalepAcıklaması}</p>
                   <p>Talep Miktarı:{entity.TalepMiktari}{entity.TalepBirim}</p>
                   <p>Toplam Tahmini Fiyat :{entity.TalepTahminiFiyat} {entity.TalepFiyatBirim}dir</p>");
        }

        private string GetTalepOnaylayanYetkiliPersonelById(int talepOnaylayanYetkiliPersonelId)
        {
            var per = _context.Personnels.FirstOrDefault(x => x.PersonID == talepOnaylayanYetkiliPersonelId );
            
            return per == null ? "" : per.PersonName;
        }

    

       

        private string GetTalepEdenPersonelRolIdById(int talepEdenPersonelRolId)
        {
            return _context.Role.FirstOrDefault(x => x.RolID == talepEdenPersonelRolId).RolName;
        }



        private string GetTalepEdenPersonelDepIdById(int talepEdenPersonelDepId)
        {
            return _context.Departments.FirstOrDefault(x => x.DepartID == talepEdenPersonelDepId).DepartName;
        }

        private string GetTalepEdenPersonelCompIdById(int talepEdenPersonelCompId)
        {
            return _context.Companies.FirstOrDefault(x => x.CompanyID == talepEdenPersonelCompId).CompanyName;
        }

        private Personnels GetTalepEdenPersonelById(int talepEdenPersonelId)
        {
            return _context.Personnels.FirstOrDefault(x => x.PersonID == talepEdenPersonelId);
        }

        // GET api/<TaleplerController>/5
        [HttpGet("{id}")]
        public Talepler Get(int id)
        {
            return _context.Talepler.FirstOrDefault(x => x.TalepID == id);

        }

        // POST api/<TaleplerController>
        [HttpPost]
        public void Post([FromBody] Talepler talepler)
        {
            var onaylayacakMail = _context.Personnels.FirstOrDefault(x => x.PersonID == talepler.TalepOnaylayanYetkiliPersonelId).PersonEmail;
            var talepEden = _context.Personnels.FirstOrDefault(x => x.PersonID == talepler.TalepEdenPersonelId).PersonName;
            talepler.TalepOnaylayanYetkiliPersonelId = 0;

            _context.Talepler.Add(talepler);
            _context.SaveChanges();

            MailService.Send(onaylayacakMail, "Talep Bildirimi",
                $@"<p>{talepEden}, personel tarafından sisteminize talep gönderilmiştir.</p>
                   <p>Taleb Adı:{talepler.TalepName}</p>
                   <p>Talep Acıklaması:{talepler.TalepAcıklaması}</p>
                   <p>Talep Miktarı:{talepler.TalepMiktari}{talepler.TalepBirim}</p>
                   <p>Toplam Tahmini Fiyat :{talepler.TalepTahminiFiyat} {talepler.TalepFiyatBirim}dir</p>");
        }

        // PUT api/<TaleplerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Talepler talepler)
        {
            var tab = _context.Talepler.FirstOrDefault(x => x.TalepID == id);

            tab.TalepID = talepler.TalepID;
            tab.TalepName = talepler.TalepName;
            tab.TalepMiktari = talepler.TalepMiktari;
            tab.TalepFiyatBirim = talepler.TalepFiyatBirim;
            tab.TalepTahminiFiyat = talepler.TalepTahminiFiyat;
            tab.TalepEdenPersonelId = talepler.TalepEdenPersonelId;
            tab.TalepEdenPersonelCompId = talepler.TalepEdenPersonelCompId;
            tab.TalepEdenPersonelDepId = talepler.TalepEdenPersonelDepId;
            tab.TalepEdenPersonelRolId = talepler.TalepEdenPersonelRolId;
            tab.TalepOnayDurum = talepler.TalepOnayDurum;
            tab.TalepOnaylayanYetkiliPersonelId = talepler.TalepOnaylayanYetkiliPersonelId;
            tab.TalepDurumu = talepler.TalepDurumu;
            tab.TalepOnayDate = talepler.TalepOnayDate;


            _context.SaveChanges();
        }


    }
}
