namespace MYPRODUCTPRO.Models
{
    public class ProductSearchViewModel
    {

        public ProductModelClass SearchModel {  get; set; } = new ProductModelClass();
         public IEnumerable<ProductModelClass> ProductsList { get; set; } = new List<ProductModelClass>();    

    }
}
