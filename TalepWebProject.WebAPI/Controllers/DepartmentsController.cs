using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalepWebProject.Entities;
using TalepWebProject.Entities.Models;
using TalepWebProject.WebAPI.ModelView;

namespace TalepWebProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {

        private readonly TalepDbContext _context;

        public DepartmentsController(TalepDbContext context)
        {
            _context = context;
        }
        // GET: api/<DepartmentsController>
        [HttpGet]
        public IEnumerable<DepartmentViewModel> Get()
        {
            var List = _context.Departments.OrderByDescending(x=>x.DepartID).ToList();

            var Departments = new Departments();
            var DepartList = new List<DepartmentViewModel>();
            foreach (var item in List)
            {
                DepartList.Add(new DepartmentViewModel
                {

                    DepartID = item.DepartID,
                    DepartName = item.DepartName,
                    CompanyId = item.CompanyId,
                    CompanyName = GetDepartCompIdById(item.CompanyId)



                });
            }
            return DepartList;
        }

        private string GetDepartCompIdById(int CompanyId)
        {
            var c = _context.Companies.FirstOrDefault(x => x.CompanyID == CompanyId);
            return c == null ? "" : c.CompanyName;

        }




        // GET api/<DepartmentsController>/5
        [HttpGet("{id}")]
        public DepartmentViewModel Get(int id)
        {
            var item = _context.Departments.FirstOrDefault(x => x.DepartID == id);
            var DepartmanModel = new DepartmentViewModel();



            if (item != null)
            {
                DepartmanModel.DepartID = item.DepartID;
                DepartmanModel.DepartName = item.DepartName;
                DepartmanModel.CompanyId = item.CompanyId;

                //DepartmanModel.CompanyId = GetDepartCompIdById(item.CompanyId);



            };

            return DepartmanModel;

        }

        // POST api/<DepartmentsController>
        [HttpPost]
        public ResultMessage Post([FromBody] Departments departments)
        {
            if (_context.Departments.Any(x => x.DepartName == departments.DepartName && x.CompanyId == departments.CompanyId ))
                    return new ResultMessage(false, "Bu şirkette bu departman mevcut.");
            _context.Departments.Add(departments);
            _context.SaveChanges();
            return new ResultMessage(); 
        }

        // PUT api/<DepartmentsController>/5
        [HttpPut]
        public void Put([FromBody] Departments departments)
        {
            var dep = _context.Departments.FirstOrDefault(x => x.DepartID == departments.DepartID);

            dep.DepartName = departments.DepartName;
            dep.CompanyId = departments.CompanyId;


            _context.SaveChanges();
        }



        // DELETE api/<DepartmentsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _context.Departments.Remove(_context.Departments.FirstOrDefault(x => x.DepartID == id));
            _context.SaveChanges();
        }
    }
}
