using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CustomerInvoice
    {
        public CustomerInvoice()
        {
            invoiceNumber = DateTime.UtcNow.Date.Year.ToString() +
                DateTime.UtcNow.Date.Month.ToString() +
                DateTime.UtcNow.Date.Day.ToString() + Guid.NewGuid().ToString().Substring(0, 4).ToUpper() + "INV";
            Date = DateTime.Now;
            Total = 0;
        }
        public int Id { get; set; }
        [Display(Name = "رقم الفاتورة")]
        [Required]
        public string invoiceNumber { get; set; }
        [Display(Name = "اسم العميل")]
		public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        [ValidateNever]
        public Customer Customer { get; set; } // The customer associated with the receipt.
        [Display(Name = "التاريخ")]
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public List<CustomerInvoiceLine> customerInvoiceLine { get; set; } = new List<CustomerInvoiceLine>();
	}
}
