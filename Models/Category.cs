using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("«”„ «·›∆…")]
        public string Name { get; set; }
        [ValidateNever]
        public ICollection<Product> Productes { get; set; }
    }
}