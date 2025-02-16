using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CustomerInvoiceLine
    {
        [Key]
        public int customerInvoiceLineId { get; set; }

        [Display(Name = "منتج")]
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product product { get; set; }

        [Required]
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }
        [Display(Name = "السعر")]

        public decimal Price { get; set; }
        [Display(Name = "التخفيض")]
        public decimal Discount { get; set; }

        [Display(Name = " المبلغ الفرعي")]
        public decimal SubAmount { get; set; } = 0;

        [Display(Name = "Customer Invoice")]
        public int customerInvoiceId { get; set; }
        [Display(Name = "Customer Invoice")]
        [ForeignKey("customerInvoiceId")]
        [ValidateNever]
        public CustomerInvoice customerInvoice { get; set; }

    }
}
