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
            // Fetch the JWT token from Session
            if (Session["JWTToken"] != null)
            {
                try
                {
                    // Set the Authorization header with the JWT token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                    // Call the API asynchronously to get the list of users
                    var response = await client.GetAsync("users");

                    // If the response is successful, process the data
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsAsync<IEnumerable<User>>();
                        return View(data);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // Token might be expired, redirect to login
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        // Handle other response statuses accordingly
                        return new HttpStatusCodeResult(response.StatusCode, "Error retrieving data from API.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the error and show an error page or handle the exception
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error fetching users.");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        // GET: User/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Fetch JWT Token
            if (Session["JWTToken"] != null)
            {
                try
                {
                    // Set the Authorization header with the JWT token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                    // Call the API asynchronously to get the user details
                    var response = await client.GetAsync($"users/{id}");

                    // If the response is successful, process the user details
                    if (response.IsSuccessStatusCode)
                    {
                        var user = await response.Content.ReadAsAsync<User>();
                        return View(user);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // If unauthorized, redirect to login
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(response.StatusCode, "Error retrieving user data.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the error and handle exceptions accordingly
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error fetching user details.");
                }
            }

            return RedirectToAction("Login");
        }


        // GET: User/SignUp
        public ActionResult SignUp()
        {
            return View();
        }

        // POST: User/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(User user)
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
        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == loginModel.Email))
                {
                    // Call the API to log in and get the token
                    var response = await client.PostAsJsonAsync("users/login", loginModel);

                    if (response.IsSuccessStatusCode)
                    {
                        // Deserialize the response into the TokenResponse class
                        var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();

                        // Store the token in session for future use
                        Session["JWTToken"] = tokenResponse.token;

                        // Extract claims from the token
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(tokenResponse.token);

                        // Store claims in session
                        Session["Firstname"] = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value; 
                        Session["Lastname"] = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value;

                        // Set the Authorization header with the token for subsequent requests
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.token);

                        return RedirectToAction("Index"); // Redirect to the user list or dashboard
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

        public class TokenResponse
        {
            public string token { get; set; }
        }

    }
}
