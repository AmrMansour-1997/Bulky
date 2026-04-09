using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWebRazor_Temp.Models
{
        public class Category
        {
            [Key]
            public int ID { get; set; }

            [Required]
            [DisplayName("Category Name")]
            public string Name { get; set; }

            [DisplayName("Display Order")]
            [Range(1, 100, ErrorMessage = "Display Order Must Be From 1-100")]
            public int DisplayOrder { get; set; }
        }
}
