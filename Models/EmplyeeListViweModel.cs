namespace MYPRODUCTPRO.Models
{
    public class EmplyeeListViweModel
    {

      public  CreateEmployeeViewModel SearchEmpolyees {  get; set; }  = new CreateEmployeeViewModel();
      public    IEnumerable<CreateEmployeeViewModel> EmplyeeList { get; set; }= new List<CreateEmployeeViewModel>();
            

    }
}
