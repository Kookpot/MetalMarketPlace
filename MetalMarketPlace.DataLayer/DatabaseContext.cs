using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MetalMarketPlace.DataLayer
{
    public class DatabaseContext : IdentityDbContext<CompanyUser>
    {
        public DatabaseContext (DbContextOptions<DatabaseContext> options) : base(options) { }
    }
}
