using Prescriptions.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prescriptions.Infrastructure.Persistence
{
    public class PrescriptionsDbContext : DbContext
    {
        public PrescriptionsDbContext() : base("name=PrescriptionsDb") { }
        public DbSet<Prescription> Prescriptions { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Prescription>().ToTable("Prescriptions");
            modelBuilder.Entity<Prescription>().HasKey(x => x.Id);
            modelBuilder.Entity<Prescription>().Property(x => x.Code).IsRequired().HasMaxLength(100);
            base.OnModelCreating(modelBuilder);
        }
    }
}
