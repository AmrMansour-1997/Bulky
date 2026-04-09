using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class ShoppingCart
    {
        public int ID { get; set; }

        [Range(1,100,ErrorMessage ="Please Enter Quantity Ranged From 1 To 100")]
        public int Count { get; set; }

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        [ValidateNever]
        public Product Product { get; set; }

        public string ApplicationUserID { get; set; }

        [ForeignKey("ApplicationUserID")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
        public double Price { get; set; }
    }
}
