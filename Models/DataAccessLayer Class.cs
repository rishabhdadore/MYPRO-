using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;

namespace MYPRODUCTPRO.Models
{
    public class DataAccessLayer_Class
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string Connection_string;
        public DataAccessLayer_Class(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {

            Connection_string = configuration.GetConnectionString("BloggingDatabase");
            _httpContextAccessor = httpContextAccessor;
        }

        public bool UserAuthentication(UsersClass UsersClsOBJ)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(Connection_string))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_AUTHENTICATION_USER", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Email", UsersClsOBJ.UserName);
                        cmd.Parameters.AddWithValue("@Password", UsersClsOBJ.UserPassword);
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();


                        if (reader.Read())
                        {

                            int UserID = Convert.ToInt32(reader["EmployeeID"]);
                            string UserName = reader["EmployeeName"].ToString() ?? "";

                            string UserRole = reader["EmployeeRole"].ToString() ?? "";
                            string profilePictureBase64 = Convert.ToBase64String((byte[])reader["ProfilePicture"]);




                            _httpContextAccessor.HttpContext.Session.SetInt32("UserID", UserID);
                            _httpContextAccessor.HttpContext.Session.SetString("UserName", UserName);
                            _httpContextAccessor.HttpContext.Session.SetString("UserRole", UserRole);
                            _httpContextAccessor.HttpContext.Session.SetString("profilePictureBase64", profilePictureBase64);

                            return true;
                        }
                        else
                        {
                            return false;

                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }
        public int CheckDuplicateEmail(CreateEmployeeViewModel employee)
        {
            using (SqlConnection con = new SqlConnection(Connection_string))
            {
                using (SqlCommand cmd = new SqlCommand("select count(*) from Employees where Email= @Email", con))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@Email", employee.Email);

                    con.Open();
                    int aAdapter = (int)cmd.ExecuteScalar();

                    return aAdapter;



                }


            }


        }



        public bool InsertEmployee(CreateEmployeeViewModel employee)
        {


            using (SqlConnection conn = new SqlConnection(Connection_string))
            {
                using (SqlCommand cmd = new SqlCommand("InsertEmployee_SP", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EmployeeName", employee.FirstName);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Mobile", employee.Mobile);
                    cmd.Parameters.AddWithValue("@Location", employee.Location);
                    cmd.Parameters.AddWithValue("@Password", employee.Password);
                    cmd.Parameters.AddWithValue("@EmployeeRole", employee.Role);
                    cmd.Parameters.AddWithValue("@IsActive", employee.Status);

                    // Image Conversion
                    if (employee.ProfilePicture != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            employee.ProfilePicture.CopyTo(memoryStream);
                            cmd.Parameters.AddWithValue("@ProfilePicture", memoryStream.ToArray());
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ProfilePicture", DBNull.Value);
                    }

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }

        }


        public bool ProductEntry(ProductModelClass modelClassOBJ)
        {
            using (SqlConnection con = new SqlConnection(Connection_string))
            {
                using (SqlCommand cmd = new SqlCommand("SP_PRODUCT_ENTRY", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductName", modelClassOBJ.ProductName);
                    cmd.Parameters.AddWithValue("@Price", modelClassOBJ.Price);
                    cmd.Parameters.AddWithValue("@Category", modelClassOBJ.Category);



                    string sessionUserName = _httpContextAccessor.HttpContext.Session.GetString("UserName");

                    cmd.Parameters.AddWithValue("@EmployeeName", sessionUserName);


                    int UserID = _httpContextAccessor.HttpContext.Session.GetInt32("UserID") ?? 0;

                    cmd.Parameters.AddWithValue("@EmployeeID", UserID);

                    if (modelClassOBJ.ProductImage != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            modelClassOBJ.ProductImage.CopyTo(memoryStream);
                            cmd.Parameters.AddWithValue("@ProductImage", memoryStream.ToArray());
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ProfilePicture", DBNull.Value);
                    }
                    con.Open();
                    int data = cmd.ExecuteNonQuery();
                    con.Close();
                    return data > 0;
                }
            }



        }


        public List<ProductModelClass> ProductCardList()
        {
            List<ProductModelClass> ProListObj = new List<ProductModelClass>();
            using (SqlConnection con = new SqlConnection(Connection_string))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID, ProductName,Price,ProductImage FROM TBL_PRODUCTS", con))
                {
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ProductModelClass modelClass = new ProductModelClass();
                        int id = Convert.ToInt32(reader["ProductID"]);
                        modelClass.ProductName = reader["ProductName"].ToString();
                        modelClass.Price = Convert.ToInt64(reader["Price"]);

                        byte[] imageBytes = (byte[])reader["ProductImage"];
                        // Convert byte array to base64 string
                        modelClass.CardProductImage = Convert.ToBase64String(imageBytes);

                        ProListObj.Add(modelClass);


                    }
                    con.Close();

                    return ProListObj;
                }

            }
        }

        public List<ProductModelClass> ProductList()
        {

            List<ProductModelClass> ListOBJ = new List<ProductModelClass>();

            using (SqlConnection con = new SqlConnection(Connection_string))
            {


                using (SqlCommand cmd = new SqlCommand("SELECT ProductID,ProductName,Price,Category,EmployeeName,EmployeeID, CreatedAt,UpdatedBy FROM TBL_PRODUCTS ", con))
                {
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        ProductModelClass productModelClass = new ProductModelClass();

                        productModelClass.ProductID = Convert.ToInt32(reader["ProductID"]);
                        productModelClass.ProductName = reader["ProductName"].ToString() ?? "NULL";
                        productModelClass.Price = Convert.ToDecimal(reader["Price"]);
                        productModelClass.EmployeeName = reader["EmployeeName"].ToString() ?? "NULL";
                        productModelClass.Category = reader["Category"].ToString() ?? "NULL";
                        productModelClass.UpdatedBy = reader["UpdatedBy"].ToString() ?? "NULL";

                        productModelClass.EmployeeID = Convert.ToInt32(reader["EmployeeID"]);
                        productModelClass.CreatedAt = reader["CreatedAt"] != null ? Convert.ToDateTime(reader["CreatedAt"]) : null;
                        ListOBJ.Add(productModelClass);

                    }
                    con.Close();
                    return ListOBJ;
                }
            }
        }




        public List<ProductModelClass> SearchForProductList(ProductModelClass productModelClassOBJ)
        {
            List<ProductModelClass> ListOBJ = new List<ProductModelClass>();

            using (SqlConnection con = new SqlConnection(Connection_string))
            {
                using (SqlCommand cmd = new SqlCommand("SearchForProductLists", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductID", (object)productModelClassOBJ.ProductID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductName", (object)productModelClassOBJ.ProductName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeName", (object)productModelClassOBJ.EmployeeName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedAt", (object)productModelClassOBJ.CreatedAt ?? DBNull.Value);


                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListOBJ.Add(new ProductModelClass
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductName = reader["ProductName"].ToString() ?? "NULL",
                                Price = Convert.ToDecimal(reader["Price"]),
                                Category = reader["Category"].ToString() ?? "NULL",
                                EmployeeName = reader["EmployeeName"].ToString() ?? "NULL",
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null
                            });
                        }
                    }
                }
            }
            return ListOBJ;
        }



        public ProductModelClass productEditBYID(int ID)
        {
            ProductModelClass productModelClassobj = null;
            using (SqlConnection con = new SqlConnection(Connection_string))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID, ProductName, Price, Category, EmployeeName, EmployeeID, CreatedAt,ProductImage  FROM TBL_PRODUCTS WHERE ProductID = @ID", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ID", ID);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            productModelClassobj = new ProductModelClass()
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductName = reader["ProductName"].ToString() ?? "NULL",
                                Price = Convert.ToDecimal(reader["Price"]),
                                Category = reader["Category"].ToString() ?? "NULL",
                                EmployeeName = reader["EmployeeName"].ToString() ?? "NULL",
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null,
                                CardProductImage = reader["ProductImage"] != DBNull.Value
                                           ? Convert.ToBase64String((byte[])reader["ProductImage"])
                                           : null

                            };

                        }
                    }
                }


            }


            return productModelClassobj;
        }



        public bool ProductDataUpdate(ProductModelClass modelClassOBJ)
        {
            using (SqlConnection con = new SqlConnection(Connection_string))
            {
                using (SqlCommand cmd = new SqlCommand("SP_PRODUCT_Data_Update", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductName", modelClassOBJ.ProductName);
                    cmd.Parameters.AddWithValue("@Price", modelClassOBJ.Price);
                    cmd.Parameters.AddWithValue("@Category", modelClassOBJ.Category);
                    cmd.Parameters.AddWithValue("@ProductID", modelClassOBJ.ProductID);



                    string sessionUserName = _httpContextAccessor.HttpContext.Session.GetString("UserName");

                    cmd.Parameters.AddWithValue("@UpdatedBy", sessionUserName);



                    if (modelClassOBJ.ProductImage != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            modelClassOBJ.ProductImage.CopyTo(memoryStream);
                            cmd.Parameters.AddWithValue("@ProductImage", memoryStream.ToArray());
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ProductImage", DBNull.Value);
                    }
                    con.Open();
                    int data = cmd.ExecuteNonQuery();
                    con.Close();
                    return data > 0;
                }
            }



        }

        public bool ProductDeleteByID(int ID)
        {
            using (SqlConnection con = new SqlConnection(Connection_string))
            {
                using (SqlCommand cmd = new SqlCommand("SP_PRODUCT_Data_Deleted", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductID", ID);

                    con.Open();

                    int data = (int)cmd.ExecuteNonQuery();

                    

                    con.Close();

                    return data > 0;
                }

            }
        }



        public List<CreateEmployeeViewModel> EmpolyeesList()
        {
            try
            {
                List<CreateEmployeeViewModel> ListOBJ = new List<CreateEmployeeViewModel>();
                using (SqlConnection con = new SqlConnection(Connection_string))
                {


                    using (SqlCommand cmd = new SqlCommand("SELECT EmployeeID,EmployeeName,Mobile,CreatedDate,Password,Location, EmployeeRole,IsActive,Email FROM Employees ", con))
                    {
                        cmd.CommandType = CommandType.Text;

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            CreateEmployeeViewModel CreateEmployeeViewModel = new CreateEmployeeViewModel();

                            CreateEmployeeViewModel.EmployeeID = Convert.ToInt32(reader["EmployeeID"]);
                            CreateEmployeeViewModel.FirstName = reader["EmployeeName"].ToString() ?? "NULL";
                            CreateEmployeeViewModel.Mobile = reader["Mobile"].ToString() ?? "NULL";
                            CreateEmployeeViewModel.Email = reader["Email"].ToString() ?? "NULL";
                            CreateEmployeeViewModel.Status = reader["IsActive"].ToString() ?? "NULL";
                            CreateEmployeeViewModel.Role = reader["EmployeeRole"].ToString() ?? "NULL";
                            CreateEmployeeViewModel.Location = reader["Location"].ToString() ?? "NULL";
                            CreateEmployeeViewModel.Password = reader["Password"].ToString() ?? "NULL";

                            CreateEmployeeViewModel.CreatedAt = reader["CreatedDate"] != null ? Convert.ToDateTime(reader["CreatedDate"]) : null;
                            ListOBJ.Add(CreateEmployeeViewModel);

                        }
                        con.Close();
                        return ListOBJ;
                    }
                }
            }

            catch (Exception ex)
            {

                throw;
            
            
            }



        }


        public List<CreateEmployeeViewModel> SearchForEmployeesList(CreateEmployeeViewModel CreateEmpViewModelOBJ)
        {
            List<CreateEmployeeViewModel> ListOBJ = new List<CreateEmployeeViewModel>();

            using (SqlConnection con = new SqlConnection(Connection_string))
            {
                using (SqlCommand cmd = new SqlCommand("SearchForEmpLists", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EmployeeID", (object)CreateEmpViewModelOBJ.EmployeeID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeName", (object)CreateEmpViewModelOBJ.FirstName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Location", (object)CreateEmpViewModelOBJ.Location ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedDate", (object)CreateEmpViewModelOBJ.CreatedAt ?? DBNull.Value);

                    //SELECT EmployeeID,EmployeeName,Mobile,CreatedDate,Password,Location, EmployeeRole,IsActive,Email FROM Employees
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListOBJ.Add(new CreateEmployeeViewModel
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                FirstName = reader["EmployeeName"].ToString() ?? "NULL",                              
                                Email = reader["Email"].ToString() ?? "NULL",
                                Password = reader["Password"].ToString() ?? "NULL",
                                Location = reader["Location"].ToString() ?? "NULL",
                                Status = reader["IsActive"].ToString() ?? "NULL",
                                Role = reader["EmployeeRole"].ToString() ?? "NULL",
                                Mobile = reader["Mobile"].ToString() ?? "NULL",

                                CreatedAt = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : (DateTime?)null
                            });
                        }
                    }
                }
            }
            return ListOBJ;
        }



    }
}