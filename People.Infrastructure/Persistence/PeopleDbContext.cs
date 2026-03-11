using People.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace People.Infrastructure.Persistence
{
    public class PeopleDbContext : DbContext

    {
        public PeopleDbContext() : base("name=PeopleDb"){ }

        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("People");
            modelBuilder.Entity<Person>().HasKey(p => p.Id);
            modelBuilder.Entity<Person>().Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Person>().Property(x => x.LastName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Person>().Property(x => x.DocumentNumber).IsRequired().HasMaxLength(50);
            base.OnModelCreating(modelBuilder);
        }

    }
}
