using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MYPRODUCTPRO.Models;

namespace MYPRODUCTPRO.Controllers
{
    public class ProductController : BaseController
    {
        private readonly DataAccessLayer_Class _DALClassObj;

        public ProductController(DataAccessLayer_Class dataAccessClassOBJ)
        {
            this._DALClassObj = dataAccessClassOBJ;
        }




        //  Add Product 
        public IActionResult CreateProductView(int? ID)
        {
            if (!IsUserAuthenticated())
                return RedirectToAction("LoginPage", "Authentication");

            ProductModelClass model;

            if (ID == null)
            {
                model = new ProductModelClass(); // Initialize empty model
            }
            else
            {
                model = _DALClassObj.productEditBYID(ID.Value);
                if (model == null)
                {
                    TempData["Error"] = "Product not found!";
                    return RedirectToAction("ProductListView"); // Redirect to another view if product not found
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult CreateProductView(ProductModelClass productOBJ )
        {
            try
            {
                if (productOBJ.ProductID == null)
                {
                    if (!ModelState.IsValid)
                    {
                        bool isProductAdded = _DALClassObj.ProductEntry(productOBJ);

                        if (isProductAdded)
                        {
                            TempData["Product"] = "Product Added Successfully!";
                            return RedirectToAction("CreateProductView");
                        }
                        else
                        {
                            TempData["Product"] = "Failed to Add Product. Try Again!";
                        }
                    }
                    else
                    {
                        TempData["Product"] = "Please fill in all required fields.";
                    }
                }
                else
                {
                    bool isProductAdded = _DALClassObj.ProductDataUpdate(productOBJ);

                    if (isProductAdded)
                    {
                        TempData["Product"] = "Product Upadate Successfully!";
                        return RedirectToAction("CreateProductView");
                    }
                    else
                    {
                        TempData["Product"] = "Failed to Add Product. Try Again!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return View(productOBJ);
        }





        /***********************  PRODUCT LIST *********************/

        public IActionResult ProductList()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            List<ProductModelClass> products = _DALClassObj.ProductList();

            var viewModel = new ProductSearchViewModel
            {
                ProductsList = products
            };

            return View(viewModel);
        }





        /************  SearchForProductList *********************/

        [HttpPost]
        public IActionResult SearchForProductList(ProductSearchViewModel productSearchOBJ)
        {
            try
            {
                if (!IsUserAuthenticated())
                {
                    return RedirectToAction("LoginPage", "Authentication");
                }

                if (productSearchOBJ.SearchModel != null &&
    (productSearchOBJ.SearchModel.ProductID.HasValue ||  // ID check
    productSearchOBJ.SearchModel.CreatedAt.HasValue ||   // Date check
    !string.IsNullOrWhiteSpace(productSearchOBJ.SearchModel.ProductName) ||  // Name check
    !string.IsNullOrWhiteSpace(productSearchOBJ.SearchModel.EmployeeName)))  // Employee Name check
                {
                    List<ProductModelClass> data = _DALClassObj.SearchForProductList(productSearchOBJ.SearchModel);

                    if (data == null || data.Count == 0)
                    {
                        TempData["NotDataFound"] = " Data Not Available"; ;
                        
                    }
                    else
                    {
                        
                        var list = new ProductSearchViewModel()
                        {
                            ProductsList = data
                        };
                        return View("ProductList", list);

                    }
                }
                else
                {
                    TempData["Data"] = " At least one field is required.";
                }
            }
            catch (Exception ex)
            {
                TempData["Exception"] = "🚨 Error: " +ex.Message;
            }
            return RedirectToAction("ProductList", "Product");
        }

        public IActionResult ProductDeleteByID(int ID)
        {
            try
            {
                bool Data = _DALClassObj.ProductDeleteByID(ID);
                if (Data)
                {
                    TempData["ProductDelete"] = "Deleted ";
                    }
            }
            catch (Exception ex)
            {

                ViewBag["Exception"] =  ex.Message;
            }


            return RedirectToAction("ProductList");

        }


    }
}