using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TalepWebProject.Entities.Models;

namespace TalepWebProject.Entities
{
    public class TalepDbContext : DbContext
    {
        public TalepDbContext(DbContextOptions<TalepDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        public TalepDbContext() { }
        public DbSet<Personnels> Personnels { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<Companies> Companies { get; set; }


        public DbSet<Talepler> Talepler { get; set; }

        public DbSet<Role> Role { get; set; }
        public DbSet<Personel_Company_Deparment> Personel_Company_Deparment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Scans a given assembly for all types that implement IEntityTypeConfiguration, and registers each one automatically
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            modelBuilder.Entity<Personel_Company_Deparment>().HasKey(t => new { t.DepartId, t.CompanyId, t.PersonelId });
            base.OnModelCreating(modelBuilder);




        }
    }
}
