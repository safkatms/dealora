using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Dealora.Context;
using Dealora.Models;

namespace Dealora.Controllers
{
    public class UserController : Controller
    {
        private DealoraAppDbContext db = new DealoraAppDbContext();

        private HttpClient client;

        public UserController()
        {
            this.client = new HttpClient();
        }

        // GET: User
        public ActionResult Index()
        {
            client.BaseAddress = new Uri(@"http://localhost:9570/api/users");
            var response = client.GetAsync("users");
            response.Wait();

            if (response.Result.IsSuccessStatusCode)
            {
                var data = response.Result.Content.ReadAsAsync<IEnumerable<User>>().Result;
                return View(data);
            }
            else
                return HttpNotFound();
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: User/Create
        public ActionResult SignUp()
        {
            return View();
        }


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
                client.BaseAddress = new Uri(@"http://localhost:9570/api/users/signup");
                var response = client.PostAsJsonAsync("signup", user);
                response.Wait();

                if (response.Result.IsSuccessStatusCode)
                    return RedirectToAction("Index");
                else
                    return HttpNotFound();
            }

            return View(user);
        }

        public ActionResult Login()
        {
            return View();
        }


    }
}
