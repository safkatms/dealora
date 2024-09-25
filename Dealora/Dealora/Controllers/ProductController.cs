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
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(@"http://localhost:9570/api/products");
                    var response = await client.GetAsync("products");

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsAsync<IEnumerable<Product>>();
                        return View(data);
                    }
                    else
                        return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error occurred while fetching data.");
            }

        }

        // GET: Product/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                client.BaseAddress = new Uri(@"http://localhost:9570/api/products");
                var response = await client.GetAsync("products/" + id.ToString());

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<Product>();
                    return View(data);
                }
                return HttpNotFound();
            }
            catch(Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.");
            }
            
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            /*ViewBag.CategoryId = new SelectList(_dbContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.UserId = new SelectList(_dbContext.Users, "Id", "FirstName", product.UserId);*/
            ViewBag.CategoryId = new SelectList(_dbContext.Categories, "Id", "Name");
            ViewBag.UserId = new SelectList(_dbContext.Users, "Id", "FirstName");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            try
            {
                client.BaseAddress = new Uri(@"http://localhost:9570/api/addproduct");
                var response = await client.PostAsJsonAsync("addproduct", product);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return new HttpStatusCodeResult(response.StatusCode, "Unable to create the product. Please try again.");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
            
        }

        // GET: Product/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(_dbContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.UserId = new SelectList(_dbContext.Users, "Id", "FirstName", product.UserId);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Product product)
        {
            client.BaseAddress = new Uri(@"http://localhost:9570/api/updateproduct");
            var response = await client.PutAsJsonAsync("updateproduct/" + product.Id.ToString(), product);
     
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }
                
        }

        // GET: Product/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            client.BaseAddress = new Uri(@"http://localhost:9570/api/deleteproduct");
            var response = await client.DeleteAsync("deleteproduct/" + id.ToString());

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
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
