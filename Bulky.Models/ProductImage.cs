using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class ProductImage
    {
        public int ID { get; set; }

        [Required]
        public string ImageURL { get; set; }

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public Product product { get; set; }
    }
}
