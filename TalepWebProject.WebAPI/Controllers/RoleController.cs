using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TalepWebProject.Entities;
using TalepWebProject.Entities.Models;
using TalepWebProject.WebAPI.ModelView;

namespace TalepWebProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {

        private readonly TalepDbContext _context;

        public RoleController(TalepDbContext context)
        {
            _context = context;
        }
        // GET: api/<RoleController>
        [HttpGet]
        public IEnumerable<RoleViewModel> Get()
        {
            var List = _context.Role.OrderByDescending(x => x.RolID).ToList();

            var Role = new Role();
            var RoleList = new List<RoleViewModel>();
            foreach (var item in List)
            {
                RoleList.Add(new RoleViewModel
                {

                    RolID = item.RolID,
                    RolName = item.RolName

                });
            }
            return RoleList;
        }

        private string GetRolDeprtById(int DepartmentId)
        {
            var d = _context.Departments.FirstOrDefault(x => x.DepartID == DepartmentId);
            return d == null ? "" : d.DepartName;

        }
        
        // GET api/<RoleController>/5
        [HttpGet("{id}")]
        public RoleViewModel Get(int id)
        {
             
            var item = _context.Role.FirstOrDefault(x => x.RolID == id);
            var RoleModel = new RoleViewModel();



            if (item != null)
            {
                RoleModel.RolID = item.RolID;
                RoleModel.RolName = item.RolName;

            };

            return RoleModel;
        }

        // POST api/<RoleController>
        [HttpPost]
        public ResultMessage Post([FromBody] Role role)
        {

            if (_context.Role.Any(x => x.RolName == role.RolName))
                return new ResultMessage(false, "Bu yetki mevcut."); //Hatalıysa ilk parametre false, 2. parametre hata mesajı
            _context.Role.Add(role);
            _context.SaveChanges();
            return new ResultMessage();
        }

        // PUT api/<RoleController>/5
        [HttpPut]
        public void Put(int id, [FromBody] Role  role)
        {
            var rol = _context.Role.FirstOrDefault(x => x.RolID ==role.RolID);
            rol.RolName = role.RolName;
            _context.SaveChanges();
        }

        // DELETE api/<RoleController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _context.Role.Remove(_context.Role.FirstOrDefault(x => x.RolID == id));
            _context.SaveChanges();
        }

    }
}
