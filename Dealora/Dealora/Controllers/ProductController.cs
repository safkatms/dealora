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
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Dealora.Models.ViewModel;

namespace Dealora.Controllers
{
    public class ProductController : Controller
    {
        private DealoraAppDbContext _dbContext;
        private HttpClient client;

        public ProductController()
        {
            this._dbContext = new DealoraAppDbContext();
            this.client = new HttpClient();
        }

        // GET: Product
        public async Task<ActionResult> Index()
        {
            if (Session["JWTToken"] != null)
            {
                try
                {   // Set the Authorization header with the JWT token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                    int userId = (int)Session["UserId"];

                    client.BaseAddress = new Uri(@"http://localhost:9570/api/products/userId");
                    var response = await client.GetAsync("userId/" + userId.ToString());

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsAsync<IEnumerable<Product>>();
                        return View(data);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Unauthorized", "Home");
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

        // GET: Product/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (Session["JWTToken"] != null)
            {
                try
                {
                    if (Session["Type"].ToString() != "Seller")
                    {
                        return RedirectToAction("Unauthorized", "Home");
                    }

                    // Set the Authorization header with the JWT token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                    client.BaseAddress = new Uri(@"http://localhost:9570/api/products");
                    var response = await client.GetAsync("products/" + id.ToString());

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsAsync<Product>();
                        return View(data);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Unauthorized", "Home");
                    }
                    else
                    return HttpNotFound();
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
               
            
        }

        // GET: Product/Create
        public async Task<ActionResult> Create()
        {

            if (Session["JWTToken"] != null)
            {
                if (Session["Type"].ToString() != "Seller")
                {
                    return RedirectToAction("Unauthorized", "Home");
                }

                // Set the Authorization header with the JWT token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                // Fetch categories through API
                client.BaseAddress = new Uri(@"http://localhost:9570/api/");
                var response = await client.GetAsync("categories");

                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadAsAsync<IEnumerable<Category>>();
                    ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
                }
                else
                {
                    return new HttpStatusCodeResult(response.StatusCode, "Unable to fetch categories.");
                }

                TempData["UserId"] = (int)Session["UserId"];
                TempData["FirstName"] = (string)Session["FirstName"];

                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
                
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product, HttpPostedFileBase ProductImage)
        {
            
            if (Session["JWTToken"] != null)
            {
                try
                {
                    // Handle the image upload
                    if (ProductImage != null && ProductImage.ContentLength > 0)
                    {
                        // Generate filename
                        string fileName = Path.GetFileNameWithoutExtension(ProductImage.FileName);
                        string extension = Path.GetExtension(ProductImage.FileName);
                        fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                        //Setpath 
                        string path = Path.Combine(Server.MapPath("~/Product_Image"), fileName);

                        // Save img in server
                        ProductImage.SaveAs(path);

                        //pass img urld in db
                        product.ImageUrl = "/Product_Image/" + fileName;
                    }

                    // Set the Authorization header with the JWT token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                    client.BaseAddress = new Uri(@"http://localhost:9570/api/addproduct");
                    var response = await client.PostAsJsonAsync("addproduct", product);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["msgAdd"] = "Product Information added sucessfully!";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(response.StatusCode, "Unable to create the product. Please try again.");
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

        // GET: Product/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (Session["JWTToken"] != null)
            {
                if (Session["Type"].ToString() != "Seller")
                {
                    return RedirectToAction("Unauthorized", "Home");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());
                client.BaseAddress = new Uri(@"http://localhost:9570/api/");

                // Fetch product for view
                var productResponse = await client.GetAsync("products/" + id.ToString());
                if (productResponse.IsSuccessStatusCode)
                {
                    var product = await productResponse.Content.ReadAsAsync<Product>();
                    if (product == null)
                    {
                        return HttpNotFound();
                    }

                    // Fetch categories through API
                    var response = await client.GetAsync("categories");
                    if (response.IsSuccessStatusCode)
                    {
                        var categories = await response.Content.ReadAsAsync<IEnumerable<Category>>();

                        ViewBag.CategoryId = new SelectList(categories, "Id", "Name", product.CategoryId);
                        //Console.WriteLine(product.CategoryId);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(response.StatusCode, "Unable to fetch categories.");
                    }

                    TempData["UserId"] = (int)Session["UserId"];
                    TempData["FirstName"] = (string)Session["FirstName"];

                    return View(product);
                }
                else
                {
                    return new HttpStatusCodeResult(productResponse.StatusCode, "Unable to fetch product.");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }


        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Product product)
        {
            if (Session["JWTToken"] != null)
            {
                // Set the Authorization header with the JWT token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                client.BaseAddress = new Uri(@"http://localhost:9570/api/updateproduct");
                var response = await client.PutAsJsonAsync("updateproduct/" + product.Id.ToString(), product);

                if (response.IsSuccessStatusCode)
                {
                    TempData["msgAdd"] = "Product Information Updated sucessfully!";

                    return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        // GET: Product/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (Session["JWTToken"] != null)
            {
                if (Session["Type"].ToString() != "Seller")
                {
                    return RedirectToAction("Unauthorized", "Home");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                client.BaseAddress = new Uri(@"http://localhost:9570/api/");
                var productResponse = await client.GetAsync("products/" + id.ToString());

                if (productResponse.IsSuccessStatusCode)
                {
                    // Read as a single product
                    var product = await productResponse.Content.ReadAsAsync<Product>();

                    if (product == null)
                    {
                        return HttpNotFound();
                    }

                    TempData["msgAdd"] = "Are you sure you want to delete this Product ?";
                    return View(product);
                }
                else
                {
                    return new HttpStatusCodeResult(productResponse.StatusCode, "Unable to fetch product.");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (Session["JWTToken"] != null)
            {
                // Set the Authorization header with the JWT token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                client.BaseAddress = new Uri(@"http://localhost:9570/api/deleteproduct");
                var response = await client.DeleteAsync("deleteproduct/" + id.ToString());

                if (response.IsSuccessStatusCode)
                {
                    TempData["msgAdd"] = "Product deleted sucessfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }


        }

        public async Task<ActionResult> SellerDashboard()
        {
            if (Session["JWTToken"] != null)
            {
                try
                {
                    // Set the Authorization header with the JWT token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                    int userId = (int)Session["UserId"];

                    // Call the API to get the seller's dashboard data
                    client.BaseAddress = new Uri(@"http://localhost:9570/api/seller/dashboard/");
                    var response = await client.GetAsync(userId.ToString());

                    if (response.IsSuccessStatusCode)
                    {
                        var dashboardData = await response.Content.ReadAsAsync<IEnumerable<dynamic>>(); // Use dynamic
                        return View(dashboardData);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Unauthorized", "Home");
                    }
                    else
                    {
                        return HttpNotFound();
                    }
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
