using Dealora.Context;
using Dealora.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dealora.Controllers
{
    public class DiscountController : Controller
    {
        private readonly DealoraAppDbContext _db = new DealoraAppDbContext();
        private readonly HttpClient client;

        public DiscountController()
        {
            this.client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:9570/api/")
            };
        }

        // GET: Discounts
        public async Task<ActionResult> Index()
        {
            if (Session["JWTToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                var response = await client.GetAsync("discounts");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<IEnumerable<Discount>>();
                    return View(data);
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Unauthorized", "Home");
                }

                return new HttpStatusCodeResult(response.StatusCode, "Error retrieving data from API.");
            }

            return RedirectToAction("Login", "User");
        }

        // POST: Discounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Discount discount)
        {
            if (ModelState.IsValid)
            {
                var response = await client.PostAsJsonAsync("discounts", discount);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                // Handle API errors more explicitly
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ModelState.AddModelError("", "Invalid data provided.");
                }
                else
                {
                    return new HttpStatusCodeResult(response.StatusCode, "Error creating discount.");
                }
            }

            return View(discount);
        }

        // Dispose HttpClient
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                client.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
