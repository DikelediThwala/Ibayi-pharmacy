using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ONT_PROJECT.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public object Prescriptions { get; internal set; }
        public DbSet<B_Order> B_Orders { get; set; }
    }
}
