using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using ThetHtarThuZArDotNetCore.MvcApp.Data;
using ThetHtarThuZArDotNetCore.MvcApp.Models.Entities;
using ThetHtarThuZArDotNetCore.MvcApp.Models.ResponseModel;

namespace ThetHtarThuZArDotNetCore.MvcApp.Controllers;

public class CreateController : Controller
{
    #region Initializations

    private readonly IConfiguration _configuration;

    public CreateController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    #endregion

    #region Index

    public IActionResult Index()
    {
        return View();
    }

    #endregion

    #region Login Page

    [ActionName("LoginPage")]
    public IActionResult GotoLoginPage()
    {
        return View();
    }

    #endregion

    #region Login

    [HttpPost]
    public IActionResult Login(UserModel dataModel)
    {
        try
        {
            SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
            conn.Open();
            string query = @"SELECT [UserId]
      ,[UserName]
      ,[Email]
      ,[UserRole]
      ,[IsActive]
  FROM [dbo].[Users] WHERE UserName = @UserName AND Email = @Email AND IsActive = @IsActive";
            SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserName", dataModel.UserName);
            cmd.Parameters.AddWithValue("@Email", dataModel.Email);
            cmd.Parameters.AddWithValue("@IsActive", true);
            SqlDataAdapter adapter = new(cmd);
            DataTable dt = new();
            adapter.Fill(dt);

            conn.Close();

            if (dt.Rows.Count > 0)
            {
                TempData["successMessage"] = "Login Successful!";
                HttpContext.Session.SetString("UserId", dt.Rows[0]["UserId"].ToString()!); // set session
               /* StaticData.UserId = Convert.ToInt64(dt.Rows[0]["UserId"]);*/ // set id using setter

                return RedirectToAction("UserManagement");
            }
            TempData["fail"] = "Login Fail!";

            return RedirectToAction("LoginPage");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region User Management

    public IActionResult UserManagement()
    {
        try
        {
            //#region Validate user

            //if (StaticData.UserId == 0) // get id using getter
            //{
            //    TempData["fail"] = "Login Fail!";
            //    return RedirectToAction("LoginPage");
            //}

            //#endregion

            SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
            conn.Open();
            string query = @"SELECT [UserId]
      ,[UserName]
      ,[Email]
      ,[UserRole]
      ,[IsActive]
  FROM [dbo].[Users] WHERE  IsActive = @IsActive";
            SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("IsActive", true);
            SqlDataAdapter adapter = new(cmd);
            DataTable dt = new();
            adapter.Fill(dt);
            conn.Close();

            string jsonStr = JsonConvert.SerializeObject(dt);//convert to json
            List<UserResponseModel> lst = JsonConvert.DeserializeObject<List<UserResponseModel>>(jsonStr)!;//convert to DataModel

            return View(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region Edit User

    public IActionResult EditUser(long id)
    {
        try
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                TempData["error"] = "Please login first!";
                return RedirectToAction("LoginPage");
            }

            SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
            conn.Open();
            string query = @"SELECT [UserId]
                    ,[UserName]
                   ,[Email]
                    ,[UserRole]
                    ,[IsActive]
                FROM [dbo].[Users] WHERE UserId = @UserId AND IsActive = @IsActive";
            SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserId", id);
            cmd.Parameters.AddWithValue("@IsActive", true);
            SqlDataAdapter adapter = new(cmd);
            DataTable dt = new();
            adapter.Fill(dt);
            conn.Close();

            if (dt.Rows.Count == 0)
            {
                TempData["error"] = "No data found!";

            }
            string jsonStr = JsonConvert.SerializeObject(dt);
            List<UserModel> user = JsonConvert.DeserializeObject<List<UserModel>>(jsonStr)!;

            return View(user);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region Update User

    [HttpPost]
    public IActionResult Update(UpdateUserResponseModel requestModel)
    {
        try
        {
            SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
            conn.Open();
            string query = @"UPDATE Users SET UserName = @UserName, Email = @Email
WHERE UserId = @UserId AND IsActive = @IsActive ";
            SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserId", requestModel.UserId);
            cmd.Parameters.AddWithValue("@UserName", requestModel.UserName);
            cmd.Parameters.AddWithValue("@Email", requestModel.Email);
            cmd.Parameters.AddWithValue("@IsActive", true);
            int result = cmd.ExecuteNonQuery();
            conn.Close();

            if (result > 0)
            {
                TempData["success"] = "Updating Successful!";
            }
            else
            {
                TempData["error"] = "Updating Fail!";
            }

            return RedirectToAction("UserManagement");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region Go to Create User Page

    [ActionName("UserCreatePage")]

    public IActionResult GotoUserCreatePage()
    {
        return View();
    }

    #endregion

    #region Create

    [HttpPost]
    public IActionResult Create(UserModel dataModel)
    {
        try
        {
            if (string.IsNullOrEmpty(dataModel.UserName) || string.IsNullOrEmpty(dataModel.Email))
            {
                TempData["error"] = "Please fill all fields......";
                return RedirectToAction("UserManagement");
            }

            if (IsEmailDuplicate(dataModel.Email))
            {
                TempData["error"] = "User with this email already exists!";
                return RedirectToAction("UserManagement");
            }

            SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
            conn.Open();
            string query = @"INSERT INTO Users (UserName,Email, UserRole, IsActive) 
VALUES(@UserName, @Email, @UserRole,@IsActive)";
            SqlCommand cmd = new(query, conn);
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@UserName", dataModel.UserName),
                new SqlParameter("@Email", dataModel.Email),
                new SqlParameter("@UserRole", EnumUserRoles.User.ToString()),
                new SqlParameter("@IsActive", true)
            };
            cmd.Parameters.AddRange(parameters.ToArray());

            int result = cmd.ExecuteNonQuery();

            conn.Close();

            if (result > 0)
            {
                TempData["success"] = "Creating Successful!";
            }
            else
            {
                TempData["error"] = "Creating Fail!";
            }

            return RedirectToAction("UserManagement");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region Delete

    public IActionResult Delete(long id)
    {
        try
        {
            SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
            conn.Open();
            string query = @"UPDATE Users SET IsActive = @IsActive WHERE UserId =  @UserId";
            SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserId", id);
            cmd.Parameters.AddWithValue("@IsActive", false);
            int result = cmd.ExecuteNonQuery();
            conn.Close();

            if (result > 0)
            {
                TempData["success"] = "Deleting Successful!";
            }
            else
            {
                TempData["error"] = "Deleting Fail!";
            }

            return RedirectToAction("UserManagement");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region Is Duplicate

    private bool IsEmailDuplicate(string email)
    {
        try
        {
            SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
            conn.Open();
            string query = @"SELECT [UserId]
      ,[UserName]
      ,[Email]
      ,[UserRole]
      ,[IsActive]
  FROM [dbo].[Users] WHERE Email = @Email AND IsActive = @IsActive";

            SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@IsActive", true);
            SqlDataAdapter adapter = new(cmd);
            DataTable dt = new();
            adapter.Fill(dt);
            conn.Close();

            return dt.Rows.Count > 0;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion
}

#region Enum User Roles

public enum EnumUserRoles
{
    None,
    User,
    Admin
}

#endregion