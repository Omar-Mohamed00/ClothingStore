using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
	public class CustomerInvoiceVM
    {
		public CustomerInvoice CustomerInvoice { get; set; }
        [ValidateNever]
		public CustomerInvoiceLine CustomerInvoiceLine { get; set; }
        [ValidateNever]
		public IEnumerable<CustomerInvoiceLine> CustomerInvoiceList { get; set; }
		[ValidateNever]
        public IEnumerable<SelectListItem> CustomerList { get; set; }
        [ValidateNever]
		public IEnumerable<SelectListItem> MedicineList { get; set; }
	}
}
