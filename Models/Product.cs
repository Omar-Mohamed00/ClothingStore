using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("اسم المنتج")]
		public string Name { get; set; }
        [DisplayName("الكمية")]
		public int Quantity { get; set; }
        [DisplayName("سعر الشراء")]
		public decimal PurchasePrice { get; set; }
        [DisplayName("سعر البيع")]
		public decimal SellingPrice { get; set; }
        [Required]
        [DisplayName("المقاس")]
		public string? Size { get; set; } // S, M, L, XL, XXL
        [Required]
        [DisplayName("اللون")]
		public string? Color { get; set; }
        [Required(ErrorMessage = "Category is required.")]
        [DisplayName("الفئة")]
		public int? CategoryId { get; set; } // Foreign key
        [DisplayName("الفئة")]
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
    }
}
