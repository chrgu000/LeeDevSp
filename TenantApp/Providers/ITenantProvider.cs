using TenantApp.Models;

namespace TenantApp.Providers
{
    public interface ITenantProvider
    {
        Tenant GetTenant();
    }
}