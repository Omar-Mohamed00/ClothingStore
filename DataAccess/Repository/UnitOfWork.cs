using DataAccess.Data;
using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public ICategoryRepository Category { get; private set; }
        public ICustomerRepository Customer{ get; private set; }
        public IProductRepository Product { get; private set; }
        public IReceiptRepository Receipt{ get; private set; }

        public ICustomerInvoiceLineRepository CustomerInvoiceLine { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Customer = new CustomerRepository(_db);
            Product = new ProductRepository(_db);
            Receipt= new ReceiptRepository(_db);
            CustomerInvoiceLine = new CustomerInvoiceLineRepository(_db);
        }

        public void save()
        {
            _db.SaveChanges();
        }
    }
}
