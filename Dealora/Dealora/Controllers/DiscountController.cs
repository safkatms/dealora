using Dealora.Context;
using Dealora.Models;
using Dealora.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<ActionResult> Index()
        {
            if (Session["JWTToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

                var response = await client.GetAsync("discounts");

                if (response.IsSuccessStatusCode)
                {
                    var discounts = await response.Content.ReadAsAsync<IEnumerable<Discount>>();
                    var viewModel = new DiscountViewModel
                    {
                        Discounts = discounts,
                        NewDiscount = new Discount() // Empty discount object for form binding
                    };
                    return View(viewModel);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Unauthorized", "Home");
                }
                else
                {
                    return new HttpStatusCodeResult(response.StatusCode, "Error retrieving data from API.");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DiscountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingDiscount = _db.Discounts.FirstOrDefault(u => u.Code.Equals(model.NewDiscount.Code, StringComparison.OrdinalIgnoreCase));

                if (existingDiscount != null)
                {
                    ModelState.AddModelError("NewDiscount.Code", "Code is already registered.");
                    return View("Index", model);
                }


                var response = await client.PostAsJsonAsync("discounts", model.NewDiscount);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound();
                }
            }

            return View("Index", model);
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
