using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Dealora.Context;
using Dealora.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web.Configuration;
using System.IO;
using Dealora.Models.ViewModel;

namespace Dealora.Controllers
{
    public class CategoriesController : Controller
    {
        private DealoraAppDbContext _dbContext;
        private HttpClient client;

        public CategoriesController()
        {
            this._dbContext = new DealoraAppDbContext();
            this.client = new HttpClient();
        }
        // GET: Categories
        public async Task<ActionResult> Index()
        {
            if (Session["JWTToken"] != null)
            {
                try
                {
                    // Set the Authorization header with the JWT token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                    client.BaseAddress = new Uri(@"http://localhost:9570/api/");
                    var response = await client.GetAsync("Categories");

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsAsync<IEnumerable<Category>>();
                        return View(data);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Login", "User");
                    }
                    else
                        return HttpNotFound();

                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error occurred while fetching data.");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        // GET: Categories/Create
        public async Task<ActionResult> Create()
        {

            var viewModel = new CategoryViewModel
            {
                Categories = await _dbContext.Categories.ToListAsync(),
                NewCategory = new Category()
            };
            return View(viewModel);
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryViewModel categoryViewModel)
        {
            if (Session["JWTToken"] != null)
            {
                try
                {
                    // Set the Authorization header with the JWT token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                    client.BaseAddress = new Uri(@"http://localhost:9570/api/");
                    var response = await client.PostAsJsonAsync("addcategories", categoryViewModel.NewCategory);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(response.StatusCode, "Unable to create the Category. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An unexpected error occurred: " + ex.Message);
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
