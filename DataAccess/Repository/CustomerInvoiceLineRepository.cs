using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class CustomerInvoiceLineRepository : Repository<CustomerInvoiceLine>,ICustomerInvoiceLineRepository
    {
        private readonly ApplicationDbContext _db;
        public CustomerInvoiceLineRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CustomerInvoiceLine obj)
        {
            _db.CustomerInvoiceLines.Update(obj);
        }

    }
}
