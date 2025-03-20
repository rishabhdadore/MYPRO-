using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MYPRODUCTPRO.Models;

namespace MYPRODUCTPRO.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly DataAccessLayer_Class DAL_Class_OBJ;

            public AuthenticationController (DataAccessLayer_Class DAL_Class) 
        {
            DAL_Class_OBJ = DAL_Class;

        }
        
            
        


        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginPage( UsersClass usersOBJ)
        
        {

            ViewBag.Profile = HttpContext.Session.GetString("Profile");
            try
            {
                if (usersOBJ.UserPassword != null && usersOBJ.UserName != null)
                {
                    bool user = DAL_Class_OBJ.UserAuthentication(usersOBJ);

                    if (user)
                    {
                        // DashBoardPage are Home Page and user ineterface

                        return RedirectToAction("DashBoardPage","DashBoard");
                    }
                    else {

                        ViewBag.ERROR = "Please Try Again";
                    
                    }
                }
            }
            catch (Exception EX)
            {

                ViewBag.ERROR =  new Exception(EX.Message) ;
            }
            return View();
        }
    }
}
