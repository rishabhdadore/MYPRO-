using MYPRODUCTPRO.Models;
using System.ComponentModel.DataAnnotations;

namespace MYPRODUCTPRO.Models
{
    public class ProductModelClass
    {
        public int ?ProductID { get; set; }


        public string? ProductName { get; set; }
       
        public decimal Price { get; set; }
     
        public string Category { get; set; }
       
        public IFormFile ProductImage { get; set; }

        public string EmployeeName { get; set; }

        public int EmployeeID { get; set; }

        public DateTime? CreatedAt { get; set; }


        public string UpdatedBy { get; set; }
        public string CardProductImage { get; set; }

    }
}

