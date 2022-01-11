using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TalepWebProject.Entities.Models;
using TalepWebProject.MVC.Models;
using TalepWebProject.MVC.Filter;
using System.Net.Mail;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using TalepWebProject.WebAPI.ModelView;

namespace TalepWebProject.MVC.Controllers
{
    [UserFilter]

    public class YetkiliController : Controller
    {
        private string url = "https://localhost:44356/api/";

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult TalepOlustur()
        {
            var PersonelID = HttpContext.Session.GetInt32("PersonId");
            using (HttpClient client = new HttpClient())
            {
                var person_sirket = client.GetFromJsonAsync<List<Personel_Company_Deparment>>(url + "Personnels/PersonelSirket/" + PersonelID).Result;

                var comp = client.GetFromJsonAsync<List<Companies>>(url + "Companies").Result;
                var dept = client.GetFromJsonAsync<List<Departments>>(url + "Departments").Result;


                List<Personel_SirketView> result = new List<Personel_SirketView>();
                foreach (var item in person_sirket)
                {
                    result.Add(new Personel_SirketView()
                    {
                        CompanyName = comp.FirstOrDefault(x => x.CompanyID == item.CompanyId).CompanyName,
                        CompanyId = item.CompanyId,
                        DepartId = item.DepartId,
                        DepartName = dept.FirstOrDefault(x => x.DepartID == item.DepartId).DepartName
                    });
                }

                ViewBag.comList = result;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TalepOlustur(TalepModel T)
        {
            try
            {
                var yetkiPersonel = HttpContext.Session.GetString("PersonelYetki");
                var PersonelID = HttpContext.Session.GetInt32("PersonId");
                var mail = HttpContext.Session.GetInt32("PersonelEmail");

                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    if (yetkiPersonel != null && yetkiPersonel == "3")
                    {
                        Talepler talep = new Talepler();
                        talep.TalepEdenPersonelDepId = Convert.ToInt32(T.TalepEdenPersonelCompId.Split('-')[1]);
                        talep.TalepEdenPersonelCompId = Convert.ToInt32(T.TalepEdenPersonelCompId.Split('-')[0]);
                        talep.TalepName = T.TalepName;
                        talep.TalepAcıklaması = T.TalepAcıklaması;
                        talep.TalepMiktari = T.TalepMiktari;
                        talep.TalepBirim = T.TalepBirim;
                        talep.TalepTahminiFiyat = T.TalepTahminiFiyat;
                        talep.TalepFiyatBirim = T.TalepFiyatBirim;
                        talep.TalepEdenPersonelId = Convert.ToInt32(PersonelID);
                        talep.TalepEdenPersonelRolId = Convert.ToInt32(yetkiPersonel);
                        //var miktar = talep.TalepTahminiFiyat;
                        
                        talep.TalepOnaylayanYetkiliPersonelId = await client.GetFromJsonAsync<int>(url + "Personnels/GetPerIdByRoleId/" + talep.TalepEdenPersonelCompId + "/" + talep.TalepEdenPersonelDepId + "/4");


                        
                

                       talep.TalepDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                        talep.TalepDurumu = "Onay Bekleniyor";

                        talep.TalepOnayDate = "";
                        talep.TalepOnayDurum = -1;


                        HttpResponseMessage httpResponseMessage = await client.PostAsJsonAsync(url + "Talepler", talep);




                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Taleplerim");
                        }
                        else
                        {
                            return RedirectToAction("TalepOlustur");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
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
                    var talepList = response.Where(x => depart.Any(d => d.DepartName == x.TalepEdenPersonelDepId) && comp.Any(c => c.CompanyName == x.TalepEdenPersonelCompId) && x.TalepOnaylayanYetkiliPersonelId == PersonelID && x.TalepOnayDurum == 0).ToList();

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
                    var talepList = response.Where(x => depart.Any(d => d.DepartName == x.TalepEdenPersonelDepId) && comp.Any(c => c.CompanyName == x.TalepEdenPersonelCompId) && x.TalepTahminiFiyat<20000 && x.TalepEdenPersonelRolId == role.FirstOrDefault(r=>r.RolID==2).RolName && x.TalepOnayDurum == -1).ToList();

                    ViewBag.talepList = talepList;
                    return View(talepList);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> TaleplerimOnaylananlarList(string startDate, string endDate)
        {
            ViewBag.username = HttpContext.Session.GetString("PersonelAdı");

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
                    var talepList = response.Where(x => x.TalepEdenPersonelId == PersonelID && x.TalepOnayDurum == 1).ToList();

                    ViewBag.talepList = talepList;
                    return View(talepList);
                }



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> TaleplerimRedList(string startDate, string endDate)
        {
            ViewBag.username = HttpContext.Session.GetString("PersonelAdı");

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
                    var talepList = response.Where(x => x.TalepEdenPersonelId == PersonelID && x.TalepOnayDurum == 0).ToList();
                    ViewBag.talepList = talepList;
                    return View(talepList);
                }



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> Taleplerim(string startDate, string endDate)
        {
            ViewBag.username = HttpContext.Session.GetString("PersonelAdı");

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
                    var talepList = response.Where(x => x.TalepEdenPersonelId == PersonelID && x.TalepOnayDurum == -1).ToList();
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
            var PersonRolId = HttpContext.Session.GetString("PersonelYetki");

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

    }


    public class TalepDurumModel
    {
        public int TalepId { get; set; }
        public int DurumId { get; set; }
    }



}
