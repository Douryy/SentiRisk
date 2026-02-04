using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SentiRisk.Models;

namespace SentiRisk.Data
{
    public class SentiRiskContext : DbContext
    {
        public SentiRiskContext (DbContextOptions<SentiRiskContext> options)
            : base(options)
        {
        }

        public DbSet<SentiRisk.Models.Role> Role { get; set; } = default!;
        public DbSet<SentiRisk.Models.User> User { get; set; } = default!;
    }
}
