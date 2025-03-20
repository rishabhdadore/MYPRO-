using Microsoft.AspNetCore.Mvc;

namespace MYPRODUCTPRO.Controllers
{
    public class BaseController : Controller
    {
        protected string UserRole => HttpContext.Session.GetString("UserRole");
        protected string UserName => HttpContext.Session.GetString("UserName");
        protected string ProfilePicture => HttpContext.Session.GetString("profilePictureBase64");

        protected bool IsUserAuthenticated()
        {
            if (string.IsNullOrEmpty(UserRole) || string.IsNullOrEmpty(UserName))
            {
                TempData["message"] = "Please Login";
                return false;
            }

            ViewBag.Userrole = UserRole;
            ViewBag.UserName = UserName;
            ViewBag.Profile = ProfilePicture;
            return true;
        }
    }

}
