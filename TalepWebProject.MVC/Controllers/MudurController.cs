using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TalepWebProject.Entities.Models;
using TalepWebProject.MVC.Filter;
using TalepWebProject.MVC.Models;
using TalepWebProject.WebAPI.ModelView;

namespace TalepWebProject.MVC.Controllers
{
    [UserFilter]

    public class MudurController : Controller
    {
        private string url = "https://localhost:44356/api/";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TalepDegistir(TalepDurumModel durum)
        {
            try
            {
                var PersonelID = HttpContext.Session.GetInt32("PersonId");
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.PostAsJsonAsync(url + "Talepler/TalepDurumDegistir/" + durum.TalepId + "/" + PersonelID, durum.DurumId);
                    return Redirect("PersonelTalepleri");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> OnaylananlarList(string startDate, string endDate)
        {
            try
            {

                var yetkiPersonel = HttpContext.Session.GetString("PersonelYetki");
                var PersonelID = HttpContext.Session.GetInt32("PersonId");

                var filter = "";
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                    filter = $"?startDate={startDate}&endDate={endDate}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetFromJsonAsync<List<TalepView>>(url + "Talepler" + filter);
                    var person_sirket = await client.GetFromJsonAsync<List<Personel_Company_Deparment>>(url + "Personnels/PersonelSirket/" + PersonelID);
                    var depart = await client.GetFromJsonAsync<List<DepartmentViewModel>>(url + "Departments");
                    depart = depart.Where(x => person_sirket.Any(y => y.DepartId == x.DepartID)).ToList();
                    var comp = await client.GetFromJsonAsync<List<Companies>>(url + "Companies");
                    comp = comp.Where(x => person_sirket.Any(y => y.CompanyId == x.CompanyID)).ToList();
                    var talepList = response.Where(x => depart.Any(d => d.DepartName == x.TalepEdenPersonelDepId) && comp.Any(c => c.CompanyName == x.TalepEdenPersonelCompId) && x.TalepOnaylayanYetkiliPersonelId == PersonelID && x.TalepOnayDurum == 1).ToList();

                    ViewBag.talepList = talepList;
                    return View(talepList);
                }



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> RedlerList(string startDate, string endDate)
        {

            try
            {

                var yetkiPersonel = HttpContext.Session.GetString("PersonelYetki");
                var PersonelID = HttpContext.Session.GetInt32("PersonId");

                var filter = "";
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                    filter = $"?startDate={startDate}&endDate={endDate}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetFromJsonAsync<List<TalepView>>(url + "Talepler" + filter);
                    var person_sirket = await client.GetFromJsonAsync<List<Personel_Company_Deparment>>(url + "Personnels/PersonelSirket/" + PersonelID);
                    var depart = await client.GetFromJsonAsync<List<DepartmentViewModel>>(url + "Departments");
                    depart = depart.Where(x => person_sirket.Any(y => y.DepartId == x.DepartID)).ToList();
                    var comp = await client.GetFromJsonAsync<List<Companies>>(url + "Companies");
                    comp = comp.Where(x => person_sirket.Any(y => y.CompanyId == x.CompanyID)).ToList();
                    var talepList = response.Where(x => depart.Any(d => d.DepartName == x.TalepEdenPersonelDepId) && comp.Any(c => c.CompanyName == x.TalepEdenPersonelCompId) && x.TalepOnaylayanYetkiliPersonelId == PersonelID && x.TalepOnayDurum ==0).ToList();

                    ViewBag.talepList = talepList;
                    return View(talepList);
                }



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<IActionResult> PersonelTalepleri(string startDate, string endDate)
        {
            try
            {
                var yetkiPersonel = HttpContext.Session.GetString("PersonelYetki");
                var PersonelID = HttpContext.Session.GetInt32("PersonId");

                var filter = "";
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                    filter = $"?startDate={startDate}&endDate={endDate}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetFromJsonAsync<List<TalepView>>(url + "Talepler" + filter);
                    var person_sirket = await client.GetFromJsonAsync<List<Personel_Company_Deparment>>(url + "Personnels/PersonelSirket/" + PersonelID);
                    var depart = await client.GetFromJsonAsync<List<DepartmentViewModel>>(url + "Departments");
                    depart = depart.Where(x => person_sirket.Any(y => y.DepartId == x.DepartID)).ToList();
                    var comp = await client.GetFromJsonAsync<List<Companies>>(url + "Companies");
                    comp = comp.Where(x => person_sirket.Any(y => y.CompanyId == x.CompanyID)).ToList();
                    var role = client.GetFromJsonAsync<List<RoleView>>(url + "Role").Result;

                    var talepList = response.Where(x => depart.Any(d => d.DepartName == x.TalepEdenPersonelDepId) && comp.Any(c => c.CompanyName == x.TalepEdenPersonelCompId) && (x.TalepTahminiFiyat >= 20000 || x.TalepEdenPersonelRolId == role.FirstOrDefault(r => r.RolID == 3).RolName) && x.TalepOnayDurum == -1).ToList();
                    ViewBag.talepList = talepList;
                    return View(talepList);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profil(int id)
        {
            var PersonelID = HttpContext.Session.GetInt32("PersonId");

            PersonelView per = new PersonelView();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url + "Personnels/" + PersonelID))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    per = JsonConvert.DeserializeObject<PersonelView>(apiResponse);
                }
            }


            return View(per);
        }


        [HttpPost]
        public async Task<IActionResult> Profil(PersonelView personelView)
        {


            try
            {
                personelView.PersonRolId = HttpContext.Session.GetString("PersonelYetki");
                //personelView.PersonID = HttpContext.Session.GetInt32("PersonId");
                personelView.PersonName = HttpContext.Session.GetString("PersonelAdı");
                personelView.PersonGender = HttpContext.Session.GetString("cinsiyet");
                personelView.PersonPhoto = HttpContext.Session.GetString("resim");

                if (personelView.PersonPhotoFile != null && personelView.PersonPhotoFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        personelView.PersonPhotoFile.CopyTo(ms);
                        personelView.PersonPhotoByte = ms.ToArray();
                    }
                }

                // personelView.PersonDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                HttpClient httpClient = new HttpClient();
                var result = httpClient.PutAsJsonAsync(url + "Personnels", personelView).Result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return RedirectToAction("Profil");


        }

        public IActionResult PasswordAgain()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PasswordAgain(PasswordAgainModel passwordAgainModel)
        {
            HttpClient httpClient = new HttpClient();
            var result = httpClient.PostAsJsonAsync(url + "Personnels/PasswordAgain", passwordAgainModel).Result;
            if (result.IsSuccessStatusCode)
            {
                return Redirect("/Home/Profil");
            }
            return View();
        }



        public class TalepDurumModel
        {
            public int TalepId { get; set; }
            public int DurumId { get; set; }
        }
    }

  

}
