
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Demo.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(15)]    [MinLength(2)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Display Order cannot be empty")]
        [Range(1,100, ErrorMessage = "Order must be between 1 and 100")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

        
    }
}
