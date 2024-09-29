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
        

        // GET: Categories/Create
        public async Task<ActionResult> Create()
        {
            if (Session["JWTToken"] == null || string.IsNullOrEmpty(Session["JWTToken"].ToString()))
            {
                return RedirectToAction("Login", "User"); // Redirect to login if not authenticated
            }
            if (Session["Type"]?.ToString() != "Admin")
            {
                return RedirectToAction("Unauthorized", "Home");
            }
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
        public async Task<ActionResult> Create(CategoryViewModel categoryViewModel, HttpPostedFileBase CategoryImage)
        {
            if (Session["JWTToken"] != null)
            {
                try
                {
                    // Handle the image upload
                    if (CategoryImage != null && CategoryImage.ContentLength > 0)
                    {
                        // Generate filename
                        string fileName = Path.GetFileNameWithoutExtension(CategoryImage.FileName);
                        string extension = Path.GetExtension(CategoryImage.FileName);
                        fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                        //Setpath 
                        string path = Path.Combine(Server.MapPath("~/Product_Image"), fileName);

                        // Save img in server
                        CategoryImage.SaveAs(path);

                        //pass img urld in db
                        categoryViewModel.NewCategory.CategoryImageUrl = "/Product_Image/" + fileName;
                    }


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
