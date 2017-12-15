using ConsoleAppEFCore.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppEFCore.Model
{
    public class EFCoreContext : DbContext
    {
        public string ConnectionString { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        public EFCoreContext()
        {
            ConnectionString = "Server=.;Database=EFCoreDb;Trusted_Connection=True;";
            Database.EnsureCreated();
            //Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(ConnectionString);            
        }              

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(pc =>
            {
                pc.ToTable("EC_Blog").HasKey(k => k.Id);
                pc.Property(p => p.Name).IsRequired();
                pc.Property(p => p.Url).IsRequired();
                pc.Property(p => p.Count).IsRequired();
            });
        }
    }
}
