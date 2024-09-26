using Dealora.Context;
using Dealora.Models;
using Dealora.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dealora.Controllers
{
    public class UserController : Controller
    {
        private DealoraAppDbContext db = new DealoraAppDbContext();
        private HttpClient client;

        public UserController()
        {
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri(@"http://localhost:9570/api/");
        }

        
        // GET: User
        public async Task<ActionResult> Index()
        {
            if (Session["JWTToken"] != null)
            {
                
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                    var response = await client.GetAsync("users");

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsAsync<IEnumerable<User>>();
                        return View(data);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(response.StatusCode, "Error retrieving data from API.");
                    }   
            }
            else
            {
                return RedirectToAction("Login");
            }
        }



        // GET: User/SignUp
        public ActionResult SignUp()
        {
            return View();
        }

        // POST: User/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(SignUpModel user)
        {
            // Check if the email is already registered
            if (db.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email is already registered.");
            }

            if (ModelState.IsValid)
            {
                // Call the API to sign up the user
                var response = client.PostAsJsonAsync("users/signup", user);
                response.Wait();

                if (response.Result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound();
                }
            }

            return View(user);
        }

        // GET: User/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel loginModel)
        {
            
                if (db.Users.Any(u => u.Email == loginModel.Email))
                {
                    var response = await client.PostAsJsonAsync("users/login", loginModel);

                    if (response.IsSuccessStatusCode)
                    {
                        var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();

                        Session["JWTToken"] = tokenResponse.token;

                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(tokenResponse.token);
                        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
                        if (userIdClaim != null)
                        {
                            Session["UserId"] = int.Parse(userIdClaim.Value);
                        }
                        Session["Firstname"] = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.GivenName).Value; 
                        Session["Lastname"] = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.FamilyName).Value;
                        Session["Type"] = jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value;

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.token);

                        return RedirectToAction("Index");
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        TempData["Invalid"] = "The email address does not exist.";
                    }
                    else
                    {
                        TempData["Invalid"] = "Invalid email or password.";
                    }
                }
                else
                {
                    TempData["Invalid"] = "The email address does not exist.";
                }
            

            return View(loginModel);
        }




        // Logout functionality
        public ActionResult Logout()
        {
            // Clear the session to log out the user
            Session.Clear();
            return RedirectToAction("Login");
        }


        public async Task<ActionResult> UserProfile()
        {
            if (Session["JWTToken"] != null && Session["UserId"] != null)
            {
                try
                {
                    // Set Authorization header with JWT token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                    // Retrieve the user ID from the session
                    int userId = (int)Session["UserId"];  // Get the stored user ID

                    // Call the API to get the user's profile details by user ID
                    var response = await client.GetAsync($"users/{userId}");

                    if (response.IsSuccessStatusCode)
                    {
                        // Deserialize the response into a User object
                        var user = await response.Content.ReadAsAsync<User>();
                        return View(user); // Pass the user data to the view
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Login"); // Redirect if unauthorized
                    }
                    else
                    {
                        return new HttpStatusCodeResult(response.StatusCode, "Error retrieving profile data.");
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error fetching profile.");
                }
            }
            else
            {
                return RedirectToAction("Login"); // Redirect to login if no token or no user ID
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditProfile()
        {
            if (Session["JWTToken"] != null)
            {
                int userId = (int)Session["UserId"];

                // Call the API to get the user's profile details by user ID
                var response = await client.GetAsync($"users/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response into a User object
                    var user = await response.Content.ReadAsAsync<User>();

                    // Convert User to UserUpdateModel
                    var userUpdateModel = new UserUpdateModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        // Add other fields as necessary
                    };

                    return View(userUpdateModel); // Pass the UserUpdateModel to the view
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    return new HttpStatusCodeResult(response.StatusCode, "Error retrieving profile data.");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("User/ChangePassword")]
        public async Task<ActionResult> EditProfile(UserUpdateModel model)
        {

            if (Session["JWTToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                int userId = (int)Session["UserId"];

                var response = await client.PutAsJsonAsync($"Users/Update/{userId}", model);
                var responseContent = await response.Content.ReadAsStringAsync(); 

                if (response.IsSuccessStatusCode)
                {
                    TempData["Pass"] = "Profile Updated.";
                    return RedirectToAction("UserProfile");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Error changing password.");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }


            return View(model);
        }



        // GET: User/ChangePassword
        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["JWTToken"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("User/ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            
                if (Session["JWTToken"] != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                    int userId = (int)Session["UserId"];

                    var response = await client.PutAsJsonAsync($"Users/ChangePassword/{userId}", model);
                    var responseContent = await response.Content.ReadAsStringAsync(); // Get response content
                    Console.WriteLine(responseContent); // Log response content

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Pass"] = "Password changed successfully!";
                        return RedirectToAction("UserProfile");
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error changing password.");
                    }
                }
                else
                {
                    return RedirectToAction("Login");
                }
            

            return View(model);
        }


        public class TokenResponse
        {
            public string token { get; set; }
        }

    }
}
