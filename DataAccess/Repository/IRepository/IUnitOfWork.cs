using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category{ get; }
        ICustomerRepository Customer{ get; } 
        ICustomerInvoiceLineRepository CustomerInvoiceLine { get; }
        IProductRepository Product { get; } 
        IReceiptRepository Receipt{ get; }
        void save();
        
    }
}
