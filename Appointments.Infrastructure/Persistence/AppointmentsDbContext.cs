using Appointments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Infrastructure.Persistence
{
    public class AppointmentsDbContext : DbContext
    {
        public AppointmentsDbContext() : base("name=AppointmentsDb") { }
        public DbSet<Appointment> Appointments { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>().ToTable("Appointments");
            modelBuilder.Entity<Appointment>().HasKey(x => x.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
