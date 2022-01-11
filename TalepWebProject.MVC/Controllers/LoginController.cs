using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TalepWebProject.Entities.Models;
using TalepWebProject.MVC.Models;

namespace TalepWebProject.MVC.Controllers
{


    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        private string url = "https://localhost:44356/api/";

        public LoginController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public IActionResult Index()
        {
            if(HttpContext.Session.GetInt32("PersonId").HasValue)
            {
                return Redirect("/Login/Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/Json");
                    var page = url + "Personnels/" + user.PersonEmail + "/" + user.PersonPass;
                    using (var response = await httpClient.GetAsync(page))
                    {
                        string token = await response.Content.ReadAsStringAsync();
                        var _Girisyapankullanici = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PersonelView>>(token);

                        if (_Girisyapankullanici != null && _Girisyapankullanici[0].PersonDurum == 1)
                        {
                            if (_Girisyapankullanici[0].MustChangePass) return Redirect("/Login/ChangePassword?RememberPassCode=" + _Girisyapankullanici[0].RememberPassCode);
                            
                            HttpContext.Session.SetInt32("PersonId", _Girisyapankullanici[0].PersonID);
                            HttpContext.Session.SetString("PersonelAdı", _Girisyapankullanici[0].PersonName);
                            HttpContext.Session.SetString("PersonelYetki", _Girisyapankullanici[0].PersonRolId);
                            HttpContext.Session.SetString("Personeltel", _Girisyapankullanici[0].PersonPhone);
                            HttpContext.Session.SetString("PersonelEmail", _Girisyapankullanici[0].PersonEmail);
                            HttpContext.Session.SetString("resim", _Girisyapankullanici[0].PersonPhoto);
                            HttpContext.Session.SetString("cinsiyet", _Girisyapankullanici[0].PersonGender);

                            if (_Girisyapankullanici[0].PersonRolId == "1") return Redirect("/Home/Index");//Admin                      

                            if (_Girisyapankullanici[0].PersonRolId == "3") return Redirect("/Yetkili/TalepOlustur");//Sorumlu Yetkili
                      
                            if (_Girisyapankullanici[0].PersonRolId == "4") return Redirect("/Mudur/PersonelTalepleri");//Mudur
                          
                            if (_Girisyapankullanici[0].PersonRolId == "2") return Redirect("/Personel/TalepOlustur");//Personel
                         
                            else return View("/Login/Index");
                                                                            
                        }
                        else
                        {
                            ViewBag.Hata = "Kullanıcı adı veya şifre hatalı!!!";
                            return View("Index");
                        }
                      
                    }

                }
            }
            catch (Exception  )
            {
                ViewBag.Hata = "Kullanıcı adı veya şifre hatalı!!!";
                return View("Index");
            }
        }

        public IActionResult ForgetPassword(string email)
        {
            HttpClient httpClient = new HttpClient();
            var result = httpClient.PostAsJsonAsync(url + "Personnels/ForgetPassword", email).Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            HttpClient httpClient = new HttpClient();
            var result = httpClient.PostAsJsonAsync(url + "Personnels/ChangePassword", changePasswordModel).Result;
            if (result.IsSuccessStatusCode)
            {
                return Redirect("/Login");
            }
            return View();
        }
   
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/Login/Index");
        }    
    }
}

