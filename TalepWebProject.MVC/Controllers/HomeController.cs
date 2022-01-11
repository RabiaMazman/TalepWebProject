using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TalepWebProject.API.ModelView;
using TalepWebProject.Entities.Models;
using TalepWebProject.MVC.Filter;
using TalepWebProject.MVC.Models;

namespace TalepWebProject.MVC.Controllers
{
    [UserFilter]
    public class HomeController : Controller
    {
        
        private readonly HttpClient _httpClient;
        private string url= "https://localhost:44356/api/";
        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

        public async Task<IActionResult> Index()
        {
            ViewBag.username = HttpContext.Session.GetString("PersonelAdı");
            try
            {
                    var response = GetPersons();
                    return View(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpGet]
        public ActionResult PersonelAdd()
        {

            //GetCompaies

            var comList = GetCompanies();
            var roleList = GetRole();
            List<SelectListItem> compitems = new List<SelectListItem>();
            compitems.Add(new SelectListItem { Text = "Şirket Seçiniz !", Value = "0" });
            foreach (var item in comList)
            {
                compitems.Add(new SelectListItem { Text = item.CompanyName, Value = item.CompanyID.ToString() });
            }
            List<SelectListItem> roleitems = new List<SelectListItem>();
            roleitems.Add(new SelectListItem { Text = "Yetki Seçiniz !", Value = "0" });
            foreach (var item in roleList)
            {
                roleitems.Add(new SelectListItem { Text = item.RolName, Value = item.RolID.ToString() });
            }
            ViewBag.CompanyList = compitems;
            ViewBag.RoleList = roleitems;

            return View();
        }

       


        [HttpPost]
        public ActionResult PersonelAdd(PersonelView personelView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //IEnumerable<Personnels> pers = null;

                    if (personelView.PersonPhotoFile != null && personelView.PersonPhotoFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            personelView.PersonPhotoFile.CopyTo(ms);
                            personelView.PersonPhotoByte = ms.ToArray();
                        }
                    }
                    personelView.PersonDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                    personelView.PersonDurum = 1;
                    HttpClient httpClient = new HttpClient();
                    var result = httpClient.PostAsJsonAsync(url + "Personnels", personelView).Result;
                    var resultMessage = result.Content.ReadAsAsync<ResultMessage>().Result;

                    if (resultMessage.IsSucceed)
                    {
                        ModelState.Clear();
                       // ViewBag.Result = "Başarıyla Kaydedildi";
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        PersonelAdd();
                        ViewBag.Result = resultMessage.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                PersonelAdd();
                ViewBag.Result = ex.Message;
            }


            return View(personelView);
        }


        [HttpGet]
        public async Task<IActionResult> UpdatePersonel(int id)
        {

            PersonelView per = new PersonelView();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url + "Personnels/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    per = JsonConvert.DeserializeObject<PersonelView>(apiResponse);
                }
            }

            //GetRole
            var rolList = GetRole();
            List<SelectListItem> rolitem = new List<SelectListItem>();
            rolitem.Add(new SelectListItem { Text = "Yetki Seçiniz !", Value = "0" });
            foreach (var item in rolList)
            {
                rolitem.Add(new SelectListItem { Text = item.RolName, Value = item.RolID.ToString() });
            }
            ViewBag.RoleList = rolitem;

            return View(per);
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePersonel(PersonelView personelView)
        {


            try
            {
                if (ModelState.IsValid)
                {
                    if (personelView.PersonPhotoFile != null && personelView.PersonPhotoFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            personelView.PersonPhotoFile.CopyTo(ms);
                            personelView.PersonPhotoByte = ms.ToArray();
                        }
                    }

                    personelView.PersonDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                    HttpClient httpClient = new HttpClient();
                    var result = httpClient.PutAsJsonAsync(url + "Personnels", personelView).Result;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return RedirectToAction("Index");

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

            //GetRole
            var rolList = GetRole();
            List<SelectListItem> rolitem = new List<SelectListItem>();
            rolitem.Add(new SelectListItem { Text = "Yetki Seçiniz !", Value = "0" });
            foreach (var item in rolList)
            {
                rolitem.Add(new SelectListItem { Text = item.RolName, Value = item.RolID.ToString() });
            }
            ViewBag.RoleList = rolitem;
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

        [HttpGet]
        public ActionResult GetDepartments(int CompanyId)
        {
            var deptLis = GetDepartment();
            return Ok(JsonConvert.SerializeObject(deptLis.Where(x => x.CompanyId == CompanyId).ToList()));
        }


        public ActionResult GetRoles(int DepartmentId)
        {
            var roleLis = GetRole();
            return Ok(JsonConvert.SerializeObject(roleLis.ToList()));
        }


        private List<CompaniesView> GetCompanies()
        {
            var companyList = new List<CompaniesView>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    companyList = client.GetFromJsonAsync<List<CompaniesView>>(url + "Companies").Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return companyList;
        }


        private List<PersonelView> GetPersons()
        {
            var personList = new List<PersonelView>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    personList = client.GetFromJsonAsync<List<PersonelView>>(url + "Personnels").Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return personList;
        }


        public IActionResult CompaniesList()
        {
            try
            {
                return View(GetCompanies());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public ActionResult CompaniesAdd()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CompaniesAdd(CompaniesView companiesView)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = new HttpClient();
                var result = httpClient.PostAsJsonAsync(url + "Companies", companiesView).Result;
                var resultMessage = result.Content.ReadAsAsync<ResultMessage>().Result;

                if (resultMessage.IsSucceed)
                {
                    companiesView = result.Content.ReadAsAsync<CompaniesView>().Result;

                    return RedirectToAction("CompaniesList");
                    //ViewBag.Result = "Başarılı Bir Sekilde Kaydedildi";
                    //ModelState.Clear();
                    //return View(new CompaniesView());

                }
                else
                {
                  
                        PersonelAdd();
                        ViewBag.Result = resultMessage.Message;
                    
                }
            }
            return View(companiesView);
        }

        public async Task<IActionResult> UpdateCompanies(int id)
        {


            CompaniesView companies = new CompaniesView();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url + "Companies/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    companies = JsonConvert.DeserializeObject<CompaniesView>(apiResponse);
                }
            }
            return View(companies);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateCompanies(CompaniesView companies)
        {
            //CompaniesView companies1 = new CompaniesView();

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(companies);
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PutAsync(url + "Companies", httpContent))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        return RedirectToAction("CompaniesList");
                        //ViewBag.Result = "Başarılı Şekilde Güncellendi";
                        //companies1 = JsonConvert.DeserializeObject<CompaniesView>(apiResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            //return View(companies1);


        }
        public IActionResult DepartmenList()
        {
            try
            {
                return View(GetDepartment());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private List<DepartmentsView> GetDepartment()
        {
            var deptList = new List<DepartmentsView>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    deptList = client.GetFromJsonAsync<List<DepartmentsView>>(url + "Departments").Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return deptList;

        }


        public ActionResult DepartmenAdd()
        {
            var comList = GetCompanies();
            List<SelectListItem> compitems = new List<SelectListItem>();
            compitems.Add(new SelectListItem { Text = "Şirket Seçiniz !", Value = "0" });
            foreach (var item in comList)
            {
                compitems.Add(new SelectListItem { Text = item.CompanyName, Value = item.CompanyID.ToString() });
            }
            ViewBag.CompanyList = compitems;


            return View();
        }

        [HttpPost]
        public ActionResult DepartmenAdd(DepartmentsView departmentsView)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = new HttpClient();
                var result = httpClient.PostAsJsonAsync(url + "Departments", departmentsView).Result;
                var resultMessage = result.Content.ReadAsAsync<ResultMessage>().Result;
                if (resultMessage.IsSucceed)
                {
                    departmentsView = result.Content.ReadAsAsync<DepartmentsView>().Result;
                    return RedirectToAction("DepartmenList");

                    //ViewBag.Result = "Başarılı Bir Sekilde Kaydedildi";
                    //ModelState.Clear();
                    //return View(new DepartmentsView());
                }
                else
                {
                    DepartmenAdd();
                    ViewBag.Result = resultMessage.Message;
                }
            }
            return View(departmentsView);
        }


        public async Task<IActionResult> UpdateDepartman(int id)
        {
            var comList = GetCompanies();
            List<SelectListItem> compitems = new List<SelectListItem>();
            compitems.Add(new SelectListItem { Text = "Şirket Seçiniz !", Value = "0" });
            foreach (var item in comList)
            {
                compitems.Add(new SelectListItem { Text = item.CompanyName, Value = item.CompanyID.ToString() });
            }
            ViewBag.CompanyList = compitems;

            DepartmentsView departmentsView = new DepartmentsView();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url + "Departments/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    departmentsView = JsonConvert.DeserializeObject<DepartmentsView>(apiResponse);
                }
            }
            return View(departmentsView);


           
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDepartman(DepartmentsView departmentsView)
        {
            DepartmentsView departmentsView1 = new DepartmentsView();

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(departmentsView);
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PutAsync(url + "Departments", httpContent))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        return RedirectToAction("DepartmenList");
                        //ViewBag.Result = "Başarılı Şekilde Güncellendi";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return View(departmentsView1);


        }

        private List<RoleView> GetRole()
        {
            var RoleList = new List<RoleView>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    RoleList = client.GetFromJsonAsync<List<RoleView>>(url + "Role").Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return RoleList;
        }
        public async Task<IActionResult> RoleList()
        {
            try
            {
                return View(GetRole());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public ActionResult RoleAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RoleAdd(RoleView roleView)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = new HttpClient();
                var result = httpClient.PostAsJsonAsync(url + "Role", roleView).Result;
                var resultMessage = result.Content.ReadAsAsync<ResultMessage>().Result;

                if (resultMessage.IsSucceed)
                {
                    roleView = result.Content.ReadAsAsync<RoleView>().Result;

                    //ViewBag.Result = "Başarılı Bir Sekilde Kaydedildi";
                    //ModelState.Clear();
                    // return View(new RoleView());
                    return RedirectToAction("RoleList");

                }
                else
                {
                    RoleAdd();
                    ViewBag.Result = resultMessage.Message;
                }
            }
            return View(roleView);
        }

        public async Task<IActionResult> UpdateRole(int id)
        {
            RoleView role = new RoleView();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url + "Role/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    role = JsonConvert.DeserializeObject<RoleView>(apiResponse);
                }
            }
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(RoleView role)
        {
            RoleView role1 = new RoleView();

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(role);
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PutAsync(url + "Role", httpContent))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        return RedirectToAction("RoleList");
                        //ViewBag.Result = "Başarılı Şekilde Güncellendi";
                        //companies1 = JsonConvert.DeserializeObject<CompaniesView>(apiResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return View(role1);


        }
        public async Task<IActionResult> PersonelSirketTanimSil(int departId, int companyId, int personId)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url + "Personnels/PersonelSirketTanimSil/" + departId + "/" + companyId + "/" + personId))
                {
                }
            }
            return RedirectToAction("PersonelSirketTanimla");
        }
        private List<Personel_Company_Deparment> GetPersonel_Company_Deparment()
        {
            var RoleList = new List<Personel_Company_Deparment>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    RoleList = client.GetFromJsonAsync<List<Personel_Company_Deparment>>(url + "Personnels/PersonelSirket/0").Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return RoleList;
        }

        public ActionResult PersonelSirketTanimla()
        {

            var comList = GetCompanies();
            var departList = GetDepartment();
            var personList = GetPersons();
            var personel_sirketList = GetPersonel_Company_Deparment();

            var personel_sirketViewList = new List<Personel_SirketView>();
            foreach (var item in personel_sirketList)
            {
                var dept = departList.FirstOrDefault(x => x.DepartID == item.DepartId);
                var pers = personList.FirstOrDefault(x => x.PersonID == item.PersonelId);
                personel_sirketViewList.Add
                    (
                        new Personel_SirketView
                        {
                            DepartId = item.DepartId,
                            CompanyId = item.CompanyId,
                            PersonId = item.PersonelId,
                            DepartName = dept.DepartName,
                            CompanyName = dept.CompanyName,
                            PersonName = pers.PersonName
                        }
                    );
            }

            ViewBag.comList = comList;
            ViewBag.personList = personList;
            ViewBag.personel_sirketViewList = personel_sirketViewList;

            return View();
        }
        [HttpPost]
        public ActionResult PersonelSirketTanimla(Personel_Company_Deparment personel_Company_Deparment)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = new HttpClient();
                var result = httpClient.PostAsJsonAsync(url + "Personnels/PersonelSirketTanimla", personel_Company_Deparment).Result;
                var resultMessage = result.Content.ReadAsAsync<ResultMessage>().Result;

                if (resultMessage.IsSucceed)
                {
                    personel_Company_Deparment = result.Content.ReadAsAsync<Personel_Company_Deparment>().Result;

                    return RedirectToAction("PersonelSirketTanimla");

                    //ViewBag.Result = "Başarılı Bir Sekilde Kaydedildi";
                    //ModelState.Clear();

                }
                else
                {
                    PersonelSirketTanimla();
                    ViewBag.Result = resultMessage.Message;
                }
            }
            return View("PersonelSirketTanimla"); ;
        }

        
        //public async Task<IActionResult> AktifPasifDegistir(DurumModel durum)
        //{
        //    try
        //    {
        //        var PersonelID = HttpContext.Session.GetInt32("PersonId");
        //        using (HttpClient client = new HttpClient())
        //        {
        //            var response = await client.PostAsJsonAsync(url + "Personnels/AktifPasif/" + PersonelID + "/" , durum.PersonDurum);
        //            return Redirect("Index");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        //public class DurumModel
        //{
        //   // public int PersonID { get; set; }
        //    public int PersonDurum { get; set; }
        //}

    }
}
