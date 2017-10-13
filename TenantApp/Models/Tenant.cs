using System;
using System.Collections.Generic;
using System.Text;

namespace TenantApp.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public string DatabaseConnectionString { get; set; }
    }
}
