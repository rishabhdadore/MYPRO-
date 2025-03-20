using Microsoft.AspNetCore.Mvc;
using MYPRODUCTPRO.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MYPRODUCTPRO.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly DataAccessLayer_Class _DALClassObj;

        public EmployeeController( DataAccessLayer_Class dataAccessLayer_)
        {
            this._DALClassObj = dataAccessLayer_;
        }



        public IActionResult CreateEmp()
        {
            if (!IsUserAuthenticated())
                return RedirectToAction("LoginPage", "Authentication");

            return View();
        }

        [HttpPost]
        public IActionResult CreateEmp(CreateEmployeeViewModel createEmployee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int isEmailExists = _DALClassObj.CheckDuplicateEmail(createEmployee);
                    if (isEmailExists <= 0)
                    {
                        bool isInserted = _DALClassObj.InsertEmployee(createEmployee);

                        if (isInserted)
                        {
                            TempData["SuccessMessage"] = "Employee Created Successfully!";
                            return RedirectToAction("CreateEmp");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Error while inserting employee!";
                        }
                    }
                    else
                    {
                        TempData["rMessage"] = "Email already exists!";
                    }
                }
                else
                {
                    TempData["rMessage"] = "Try Again!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return View(createEmployee);
        }

        [HttpGet]
        public IActionResult EmployeeList()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("LoginPage", "Authentication");
            }
            try
            {
                List<CreateEmployeeViewModel> data = _DALClassObj.EmpolyeesList();

                EmplyeeListViweModel OBJ = new EmplyeeListViweModel();


                OBJ.EmplyeeList = data;

                return View(OBJ);
            }
            catch (Exception ex)
            {

                TempData["Exception"] = ex.Message;
            }
            return View();
        }
        [HttpPost]
        public IActionResult EmployeeList(EmplyeeListViweModel EmplyeeListViweModelOBJ)
        
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("LoginPage", "Authentication");
            }
            try
            {
                if (EmplyeeListViweModelOBJ.SearchEmpolyees != null &&
                     (EmplyeeListViweModelOBJ.SearchEmpolyees.EmployeeID.HasValue ||  // ID check
                      EmplyeeListViweModelOBJ.SearchEmpolyees.CreatedAt.HasValue ||   // Date check
                        !string.IsNullOrWhiteSpace(EmplyeeListViweModelOBJ.SearchEmpolyees.FirstName) ||  // Name check
                            !string.IsNullOrWhiteSpace(EmplyeeListViweModelOBJ.SearchEmpolyees.Location)))
                {
                    List<CreateEmployeeViewModel> data = _DALClassObj.SearchForEmployeesList(EmplyeeListViweModelOBJ.SearchEmpolyees);

                    if (data ==  null || data.Count == 0)
                    {
                        TempData["NotDataFound"] = "Data Not Availabale";
                        return RedirectToAction("EmployeeList", "Employee");
                    }
                    else
                    {
                        EmplyeeListViweModel emplyeeListViweModelOBJ = new EmplyeeListViweModel();
                        emplyeeListViweModelOBJ.EmplyeeList = data;
                        return View("EmployeeList", emplyeeListViweModelOBJ);
                    }

                   


                }
                else
                {

                    TempData["required"] = "At least one field is required.";
                }
                



            }
            catch (Exception ex)
            {

                TempData["Exception"] = ex.Message;
            }
            return RedirectToAction("EmployeeList","Employee"); 
        }
        




    }
}
