using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TalepWebProject.API.ModelView;
using TalepWebProject.Entities;
using TalepWebProject.Entities.Models;
using TalepWebProject.WebAPI.Bussines;

namespace TalepWebProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PersonnelsController : ControllerBase
    {
        private readonly TalepDbContext _context;
       // private readonly IWebHostEnvironment _env;


        public PersonnelsController(TalepDbContext context)
        {

            _context = context;
            //_env = env;

        }


        [HttpGet]
        public IEnumerable<PersonnelViewModel> Get()
        {
            var List = _context.Personnels.OrderByDescending(x => x.PersonID).ToList();
            var personnels = new Personnels();
            var PersonnelList = new List<PersonnelViewModel>();
            foreach (var item in List)
            {
                PersonnelList.Add(new PersonnelViewModel
                {

                    PersonID = item.PersonID,
                    PersonName = item.PersonName,
                    PersonGender = item.PersonGender,
                    PersonPhone = item.PersonPhone,
                    PersonEmail = item.PersonEmail,
                    PersonPass = item.PersonPass,
                    PersonRolId = GetPersonRolIdById(item.PersonRolId),
                    PersonDate = item.PersonDate,
                    PersonDurum = item.PersonDurum,
                    PersonPhoto = item.PersonPhoto
                });
            }
            return PersonnelList;
        }


        [HttpGet("GetPerIdByRoleId/{companyId}/{departmanId}/{roleId}")]
        public int GetPerIdByRoleId(int companyId, int departmanId, int roleId)
        {
            var perIds = _context.Personel_Company_Deparment.Where(x => x.CompanyId == companyId && x.DepartId == departmanId).Select(x=>x.PersonelId).ToList();
            var result = _context.Personnels.FirstOrDefault(x => perIds.Contains(x.PersonID) && x.PersonRolId == roleId);
            return result == null ? -1 : result.PersonID;

        }

        private string GetPersonRolIdById(int personRolId)
        {
            return _context.Role.FirstOrDefault(x => x.RolID == personRolId)?.RolName;

        }
        
        private List<int> GetPersonRolIdsById(int personRolId)
        {
            return _context.Personnels.Where(x => x.PersonID == personRolId).Select(x => x.PersonRolId).ToList();

        }

        //private string GetPersonDepartmentIdById(int personDepartmentId)
        //{
        //    return _context.Departments.FirstOrDefault(x => x.DepartID == personDepartmentId).DepartName;
        //}

        //private string GetPersonCompanyIdById(int personCompanyId)
        //{
        //    return _context.Companies.FirstOrDefault(x => x.CompanyID == personCompanyId).CompanyName;
        //}

        [HttpGet("{PersonEmail}/{PersonPass}")]
        public ActionResult<List<Personnels>> Get(string PersonEmail, string PersonPass)
        {
            var personel = _context.Personnels.Where(personel => personel.PersonEmail.Equals(PersonEmail) && personel.PersonPass.Equals(PersonPass)).ToList();

            if (personel == null || personel.Count==0)
            {

                return BadRequest(error: new { message = "Hatalı yada eksik giriş" });
            }
            return personel;
        }

        // GET api/<PersonnelsController>/5
        [HttpGet("{id}")]
        public PersonnelViewModel Get(int id)
        {
            var item = _context.Personnels.FirstOrDefault(x => x.PersonID == id);
            var personelModel = new PersonnelViewModel();

            if (item != null)
            {
                personelModel.PersonID = item.PersonID;
                personelModel.PersonName = item.PersonName;
                personelModel.PersonGender = item.PersonGender;
                personelModel.PersonPhone = item.PersonPhone;
                personelModel.PersonEmail = item.PersonEmail;
                personelModel.PersonPass = item.PersonPass;
                personelModel.PersonRolId = GetPersonRolIdById(item.PersonRolId);
                personelModel.PersonDate = item.PersonDate;
                personelModel.PersonDurum = item.PersonDurum;
                personelModel.PersonPhoto = item.PersonPhoto;
            };

            return personelModel;


        }


        // POST api/<PersonnelsController>
        [HttpPost]
        public ResultMessage Post([FromBody] Personnels personnels)
        {

            try
            {

                if (_context.Personnels.Any(x => x.PersonEmail == personnels.PersonEmail))
                    return new ResultMessage(false, "Bu mail adresi ile kayıtlı personel mevcut."); //Hatalıysa ilk parametre false, 2. parametre hata mesajı

                if (personnels.PersonPhotoByte != null)
                {
                    var fileName = DateTime.Now.Ticks.ToString() + ".jpg";
                    var path = Path.Combine((string)AppDomain.CurrentDomain.GetData("ContentRootPath"), "wwwroot\\Photos\\" + fileName);
                    System.IO.File.WriteAllBytes(path, personnels.PersonPhotoByte);
                    personnels.PersonPhoto = fileName;
                }
            }
            catch (Exception ex)
            {
                return new ResultMessage(false, ex.Message);
            }
            personnels.MustChangePass = true;
            personnels.RememberPassCode = Guid.NewGuid().ToString();//sifre değiştirmek için
            _context.Personnels.Add(personnels);
            _context.SaveChanges();



            MailService.Send(personnels.PersonEmail, "Üyeliğiniz Oluşturuldu",
                $@"<p>Merhaba {personnels.PersonName}.</p>
                   <p>Üyeliğiniz oluşturulmuştur.</p>
                   <p>Şifreniz: {personnels.PersonPass}</p>
                   <p>Giriş: <a href='https://localhost:44326/'>tıklayın</a>.</p>");



            return new ResultMessage(); //başarılıysa bu şekilde return edilebilir.
        }

        // PUT api/<PersonnelsController>/5
        [HttpPut]
        public void Put([FromBody] Personnels personnels)
        {

            var per = _context.Personnels.FirstOrDefault(x => x.PersonID == personnels.PersonID);

            if (personnels.PersonPhotoByte != null)
            {
                var fileName = DateTime.Now.Ticks.ToString() + ".jpg";
                var path = Path.Combine((string)AppDomain.CurrentDomain.GetData("ContentRootPath"), "wwwroot\\Photos\\" + fileName);
                System.IO.File.WriteAllBytes(path, personnels.PersonPhotoByte);
                per.PersonPhoto = fileName;
            }


            per.PersonName = personnels.PersonName;
            per.PersonGender = personnels.PersonGender;
            per.PersonPhone = personnels.PersonPhone;
            per.PersonEmail = personnels.PersonEmail;
            per.PersonPass = personnels.PersonPass;
            per.PersonRolId = personnels.PersonRolId;
            per.PersonDate = per.PersonDate;
            per.PersonDurum = personnels.PersonDurum;
            _context.SaveChanges();
        }

        // DELETE api/<PersonnelsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _context.Personnels.Remove(_context.Personnels.FirstOrDefault(x => x.PersonID == id));
            _context.SaveChanges();
        }


        //[Route("SaveFile")]
        //[HttpPost]
        //public JsonResult SaveFile()
        //{
        //    try
        //    {
        //        var httpRequest = Request.Form;
        //        var postedFile = httpRequest.Files[0];
        //        string filename = postedFile.FileName;
        //        var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

        //        using (var stream = new FileStream(physicalPath, FileMode.Create))
        //        {
        //            postedFile.CopyTo(stream);
        //        }

        //        return new JsonResult(filename);
        //    }
        //    catch (Exception)
        //    {

        //        return new JsonResult("anonymous.png");
        //    }
        //}


        [HttpPost("ChangePassword")]
        public void ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            var per = _context.Personnels.FirstOrDefault(x => x.RememberPassCode == changePasswordModel.RememberPassCode);
            per.PersonPass = changePasswordModel.Password;
            per.RememberPassCode = null;
            per.MustChangePass = false;
            _context.SaveChanges();
        }
        [HttpPost("ForgetPassword")]
        public void ForgetPassword([FromBody] string email)
        {
            var per = _context.Personnels.FirstOrDefault(x => x.PersonEmail == email);
            per.RememberPassCode = Guid.NewGuid().ToString();
            _context.SaveChanges();

            MailService.Send(per.PersonEmail, "Parola Sıfırlama",
                $@"<p>Merhaba {per.PersonName}.</p>
                   <p>Parola sıfırlama talebiniz alındı.</p>
                   <p>Sıfırlamak İçin: <a href='https://localhost:44326/Login/ChangePassword?RememberPassCode={per.RememberPassCode}'>tıklayın</a>.</p>");
        }


        [HttpPost("PasswordAgain")]
        public void PasswordAgain([FromBody] PasswordAgainModel passwordAgainModel)
        {
            var per = _context.Personnels.FirstOrDefault(x => x.PersonPass == passwordAgainModel.PersonPass);
            per.PersonPass = passwordAgainModel.Password;
            per.PersonPass = passwordAgainModel.PasswordAgain;
            per.RememberPassCode = null;
            per.MustChangePass = false;
            _context.SaveChanges();
         
            MailService.Send(per.PersonEmail, "Parola Değiştirme",
                $@"<p>Merhaba {per.PersonName}.</p>
                   <p>Parolanız başarı ile Değiştirilmiştir.</p>");
        }

        //[HttpPost("Aktif")]
        //public void Aktif([FromBody] PersonnelViewModel personnelViewModel)
        //{
        //    var per = _context.Personnels.FirstOrDefault(x =>x.PersonID==personnelViewModel.PersonID);
        //    per.PersonDurum = 1;
        //    _context.SaveChanges();

        //}

        //[HttpPost("Pasiff")]
        //public void Pasif([FromBody] PersonnelViewModel personnelViewModel)
        //{
        //    var per = _context.Personnels.FirstOrDefault(x => x.PersonID == personnelViewModel.PersonID);
        //    per.PersonDurum = 0;
        //    _context.SaveChanges();

        //}



        [HttpPost("PersonelSirketTanimla")]
        public ResultMessage PersonelSirketTanimla([FromBody] Personel_Company_Deparment personel_Company_Deparment)
        {
            if (_context.Personel_Company_Deparment.Any(x => x.DepartId == personel_Company_Deparment.DepartId && x.PersonelId == personel_Company_Deparment.PersonelId && x.CompanyId == personel_Company_Deparment.CompanyId ))
                return new ResultMessage(false, "Personele bu şirket ve departman tanımlı .");
            _context.Personel_Company_Deparment.Add(personel_Company_Deparment);
            _context.SaveChanges();
            return new ResultMessage();
        }
        [HttpGet("PersonelSirket/{personId}")]
        public IEnumerable<Personel_Company_Deparment> PersonelSirket(int personId = 0)
        {
            IEnumerable<Personel_Company_Deparment> List = null;
            if (personId == 0)
                List = _context.Personel_Company_Deparment.ToList();
            else
                List = _context.Personel_Company_Deparment.Where(x=>x.PersonelId==personId).ToList();

            List.Reverse();
            return List;
        }
        [HttpGet("PersonelSirketTanimSil/{departId}/{companyId}/{personId}")]
        public void PersonelSirketTanimSil(int departId, int companyId, int personId)
        {
            _context.Personel_Company_Deparment.Remove(_context.Personel_Company_Deparment.FirstOrDefault(x => x.CompanyId == companyId && x.DepartId == departId && x.PersonelId == personId));
            _context.SaveChanges();
        }
    }
}
