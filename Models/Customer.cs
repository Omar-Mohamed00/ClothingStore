using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Customer
    {
        public int Id { get; set; }
        [DisplayName("اسم العميل")]
        public string Name { get; set; }
        [DisplayName("رقم التليفون")]
        public string? Phone { get; set; }
        [ValidateNever]
        public ICollection<CustomerInvoice> CustomerInvoices { get; set; } // A customer can have multiple receipts associated with their purchases.
    }
}
