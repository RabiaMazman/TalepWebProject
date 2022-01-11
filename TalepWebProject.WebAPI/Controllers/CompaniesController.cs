using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalepWebProject.Entities;
using TalepWebProject.Entities.Models;

namespace TalepWebProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly TalepDbContext _context;

        public CompaniesController(TalepDbContext context)
        {
            _context = context;
        }
        // GET: api/<CompaniesController>
        [HttpGet]
        public IEnumerable<Companies> Get()
        {
            var List = _context.Companies.OrderByDescending(x => x.CompanyID).ToList();
            return List;
        }

        // GET api/<CompaniesController>/5
        [HttpGet("{id}")]
        public Companies Get(int id)
        {
            return _context.Companies.FirstOrDefault(x => x.CompanyID == id);
        }

        // POST api/<CompaniesController>
        [HttpPost]
        public ResultMessage Post([FromBody] Companies companies)
        {
            if (_context.Companies.Any(x => x.CompanyName == companies.CompanyName))
                return new ResultMessage(false, "Bu şirket mevcut."); //Hatalıysa ilk parametre false, 2. parametre hata mesajı
            _context.Companies.Add(companies);
            _context.SaveChanges();
            return new ResultMessage();
        }

        // PUT api/<CompaniesController>/5
        [HttpPut]
        public void Put([FromBody] Companies companies)
        {
            var com = _context.Companies.FirstOrDefault(x => x.CompanyID == companies.CompanyID);
            com.CompanyName = companies.CompanyName;
            _context.SaveChanges();
        }

        // DELETE api/<CompaniesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _context.Companies.Remove(_context.Companies.FirstOrDefault(x => x.CompanyID == id));
            _context.SaveChanges();
        }
    }
}
