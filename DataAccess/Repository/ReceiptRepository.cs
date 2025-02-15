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
    public class ReceiptRepository : Repository<CustomerInvoice>, IReceiptRepository
    {
        private readonly ApplicationDbContext _db;
        public ReceiptRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CustomerInvoice obj)
        {
            _db.CustomerInvoices.Update(obj);
        }
    }
}
