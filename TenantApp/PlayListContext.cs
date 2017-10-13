using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TenantApp.Models;
using TenantApp.Providers;

namespace TenantApp
{
    public class PlayListContext : DbContext
    {
        private readonly Tenant _tenant;
        //public DbSet<PlayList> PlayLists { get; set; }
        //public DbSet<Song> Songs { get; set; }

        public PlayListContext(DbContextOptions<PlayListContext> options, ITenantProvider tenantProvider)
            : base(options)
        {
            _tenant = tenantProvider.GetTenant();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(_tenant.DatabaseConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
