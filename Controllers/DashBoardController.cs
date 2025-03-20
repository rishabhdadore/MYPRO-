using Microsoft.AspNetCore.Mvc;
using MYPRODUCTPRO.Models;
using System;

namespace MYPRODUCTPRO.Controllers
{
    public class DashBoardController : BaseController
    {
        private readonly DataAccessLayer_Class _dal;

        public DashBoardController(DataAccessLayer_Class dal)
        {
            _dal = dal;
        }

        // DashBoardPage are Home Page and user ineterface
        public IActionResult DashBoardPage()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("LoginPage", "Authentication");
            }
            else
            {
                List<ProductModelClass> data = _dal.ProductCardList();

                return View(data);
            }
        }
        








    }
}
