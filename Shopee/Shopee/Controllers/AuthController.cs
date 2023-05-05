using Microsoft.AspNetCore.Mvc;
using Shopee.Models;
using WebApplication1.Controllers;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using System.Text;

namespace Shopee.Controllers
{
    public class AuthController : Controller
    {
        static int otp = 0;
        public IActionResult SignUp()
        {
            var dataProtectionProvider = DataProtectionProvider.Create("MyApp");
            var protector = dataProtectionProvider.CreateProtector("MyCookie");

            if (Request.Cookies.TryGetValue("user", out var cookieValue))
            {
                var protectedData = Convert.FromBase64String(cookieValue);
                var userJson = Encoding.UTF8.GetString(protector.Unprotect(protectedData));
                var user = JsonSerializer.Deserialize<UserModel>(userJson);
                return RedirectToAction("Index", "User", new { userId = user.UserId });
            }
            return View();

        }

        public IActionResult SignIn()
        {
            Response.Cookies.Delete("user");
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ChangePassword(string email)
        {
            ViewBag.email = email;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(IFormCollection collection)
        {
            string userName = collection["name"];
            string userEmail = collection["email"];
            string password = collection["password"];
            string address = collection["address"];
            string confirmPwd = collection["confirmpassword"];

            UserModel model = new UserModel()
            {
                UserName = userName,
                UserEmail = userEmail,
                UserAddress = address,
                UserPassword = password
            };

            if (password != confirmPwd)
            {
                ViewData["err"] = "*Confirm password does not match with password";
                return View();
            }

            List<UserModel> users = (await new APICall<UserModel>().Get<UserModel>("User"));
            foreach (var user in users)
            {
                if (user.UserEmail == userEmail)
                {
                    ViewData["err"] = "*Email already registered";
                    return View();
                }
            }
            
            new APICall<UserModel>().Post("User", model);
            return RedirectToAction("SignIn");
        }

        [HttpPost]
        public async Task<ActionResult> SignIn(IFormCollection collection)
        {
            string userEmail = collection["email"];
            string password = collection["password"];

            if (userEmail == "admin@gmail.com" && password == "admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            List<UserModel> users = (await new APICall<UserModel>().Get<UserModel>("User"));
            foreach (var user in users)
            {
                if (user.UserEmail == userEmail && user.UserPassword == password)
                {

                    var dataProtectionProvider = DataProtectionProvider.Create("MyApp");
                    var protector = dataProtectionProvider.CreateProtector("MyCookie");

                    var userJson = JsonSerializer.Serialize(user);
                    var protectedData = protector.Protect(Encoding.UTF8.GetBytes(userJson));
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                    };
                    Response.Cookies.Append("user", Convert.ToBase64String(protectedData), cookieOptions);
                    return RedirectToAction("Index", "User", new { userId = user.UserId });
                }
                else if (user.UserEmail == userEmail && user.UserPassword != password)
                {
                    ViewData["err"] = "*Password is incorrect";
                    return View();
                }
            }
            ViewData["err"] = "*Email not registered";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(IFormCollection collection)
        {
            string email = collection["email"];
            Random random = new Random();
            int generatedOTP = random.Next(1000, 9999);
            new GenerateOTP().generateOTP(email, generatedOTP);
            otp = generatedOTP;
            return RedirectToAction("ChangePassword", new {email=email});
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(string email, IFormCollection collection)
        {
            int otpEntered = Convert.ToInt32(collection["otp"]);
            if (otpEntered != otp)
            {
                ViewData["err"] = "*OTP is incorrect";
                return RedirectToAction("ChangePassword", new { email = email });
            }
            else
            {
                List<UserModel> users = (await new APICall<UserModel>().Get<UserModel>("User"));
                foreach (var user in users)
                {
                    if (user.UserEmail == email)
                    {
                        user.UserPassword = collection["newpassword"];
                        new APICall<UserModel>().Put("User", user, user.UserId);
                        return RedirectToAction("SignIn");
                    }
                }
            }
            return RedirectToAction("ForgotPassword");
        }
    }
}
