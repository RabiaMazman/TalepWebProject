using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TalepWebProject.Entities.Models;
using TalepWebProject.MVC.Models;
using MimeKit;
using MailKit.Net.Smtp;
using TalepWebProject.MVC.Filter;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.IO;

namespace TalepWebProject.MVC.Controllers
{
    [UserFilter]

    public class PersonelController : Controller
    {
        private readonly HttpClient _httpClient;
        private string url = "https://localhost:44356/api/";

        public PersonelController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public IActionResult Index()
        {
            //var s = HttpContext.Session.GetString("personName");

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
                    if (yetkiPersonel != null && yetkiPersonel == "2")
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
                        var miktar = talep.TalepTahminiFiyat;
                        if (miktar != null && miktar >= 20000)
                        {
                            talep.TalepOnaylayanYetkiliPersonelId = await client.GetFromJsonAsync<int>(url + "Personnels/GetPerIdByRoleId/" + talep.TalepEdenPersonelCompId + "/" + talep.TalepEdenPersonelDepId + "/4");
                            //var plist = talep.TalepOnaylayanYetkiliPersonelId;


                        }
                        else
                        {
                            talep.TalepOnaylayanYetkiliPersonelId = await client.GetFromJsonAsync<int>(url + "Personnels/GetPerIdByRoleId/" + talep.TalepEdenPersonelCompId + "/" + talep.TalepEdenPersonelDepId + "/3");
                            //var plist = talep.TalepOnaylayanYetkiliPersonelId;
                        }

                        talep.TalepDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                        talep.TalepDurumu = "Onay Bekleniyor";

                        talep.TalepOnayDate = "";
                        talep.TalepOnayDurum = -1;


                        HttpResponseMessage httpResponseMessage = await client.PostAsJsonAsync(url + "Talepler", talep);




                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            return RedirectToAction("TaleplerimiListele");
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
        public async Task<IActionResult> TaleplerimiListele(string startDate, string endDate)
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
                    var talepList = response.Where(x => x.TalepEdenPersonelId == PersonelID && x.TalepOnayDurum == -1).ToList();
                    ViewBag.talepList = talepList;
                    return View(talepList);
                }



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            //return View();
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
}
