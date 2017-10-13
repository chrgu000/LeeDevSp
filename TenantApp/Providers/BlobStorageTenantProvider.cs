using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TenantApp.Models;

namespace TenantApp.Providers
{
    public class BlobStorageTenantProvider : ITenantProvider
    {
        private static IList<Tenant> _tenants;
        private Tenant _tenant;

        public BlobStorageTenantProvider(IHttpContextAccessor accessor, IConfiguration conf)
        {
            if (_tenants == null)
            {
                //LoadTenants(conf["StorageConnectionString"], conf["TenantsContainerName"], conf["TenantsBlobName"]);
                var host = accessor.HttpContext.Request.Host.Value;

                //conf.GetSection("Tenants").GetSection("0").GetSection("Id")

                var tenantSessions = conf.GetSection("Tenants").GetChildren().ToList();
                _tenants = new List<Tenant>();
                for (int i = 0; i < tenantSessions.Count; ++i)
                {
                    _tenants.Add(new Tenant
                    {
                        Id = int.Parse(tenantSessions[i].GetSection("Id").Value),
                        Name = tenantSessions[i].GetSection("Name").Value,
                        Host = tenantSessions[i].GetSection("Host").Value,
                        DatabaseConnectionString = tenantSessions[i].GetSection("DatabaseConnectionString").Value
                    });                    
                }

                var tenant = _tenants.FirstOrDefault(t => t.Host.Equals(host, StringComparison.OrdinalIgnoreCase));
                if (tenant != null)
                {
                    _tenant = tenant;
                }
            }
        }

        private void LoadTenants(string connStr, string containerName, string blobName)
        {
            //var storageAccount = Cloud
        }

        public Tenant GetTenant()
        {
            return _tenant;
        }
    }
}
