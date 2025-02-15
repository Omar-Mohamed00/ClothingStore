using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CustomerInvoice> CustomerInvoices { get; set; }
        public DbSet<CustomerInvoiceLine> CustomerInvoiceLines { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerInvoiceLine>(b =>
            {
                b.HasOne(cil => cil.customerInvoice)
                    .WithMany(ci => ci.customerInvoiceLine)
                    .HasForeignKey(cil => cil.customerInvoiceId)
                    .OnDelete(DeleteBehavior.Restrict);  // Avoid cascade delete for CustomerInvoice

                b.HasOne(cil => cil.product)
                    .WithMany()
                    .HasForeignKey(cil => cil.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);  // Allow cascade delete for Medicine
            });


            base.OnModelCreating(modelBuilder);
        }
	}
}
