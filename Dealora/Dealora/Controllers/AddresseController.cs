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
using Dealora.ViewModels;
using Dealora.Models.ViewModel;

namespace Dealora.Controllers
{
    public class AddresseController : Controller
    {
        private DealoraAppDbContext db = new DealoraAppDbContext();
        private HttpClient client;

        public AddresseController()
        {
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri(@"http://localhost:9570/api/");
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            if (Session["JWTToken"] != null)
            {
                // Set Authorization header with JWT token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["JWTToken"].ToString());

           
                int userId = (int)Session["UserId"];

                var response = await client.GetAsync($"Addresses/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var address = await response.Content.ReadAsAsync<Address>();
                    return View(address);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Unauthorized", "Home");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }





        // GET: Addresse/Create
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["JWTToken"] != null)
            {
                if (Session["Type"].ToString() != "Customer")
                {
                    return RedirectToAction("Unauthorized", "Home");
                }
                ViewBag.Cities = new SelectList(GetCities());
                return View();
            }
            else
            {
                return RedirectToAction("Login","User");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Address address)
        {
            if (Session["UserId"] != null)
            {
                int userId = (int)Session["UserId"];
                address.UserId = userId;  // Assign UserId to the address model

                if (ModelState.IsValid)
                {
                    var response = client.PostAsJsonAsync("addresses", address);
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

                return View(address);  // In case ModelState is invalid
            }
            else
            {
                return RedirectToAction("Login", "User");  // Redirect to login if session expired
            }
        }



        private List<string> GetCities()
        {
            return new List<string>
        {
            "Dhaka",
            "Chittagong",
            "Khulna",
            "Rajshahi",
            "Sylhet",
            "Barisal",
            "Rangpur",
            "Comilla",
            "Mymensingh"
        };
        }

        // GET: Addresse/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (Session["JWTToken"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Address address = await db.Addresses.FindAsync(id);
                if (address == null)
                {
                    return HttpNotFound();
                }
                if (Session["Type"].ToString() != "Customer" || (int)Session["UserId"] != address.User.Id)
                {
                    return RedirectToAction("Unauthorized", "Home");
                }
                // Map the Address entity to AddressUpdateModel
                var addressViewModel = new AddressUpdateModel
                {
                    Id = address.Id,
                    StreetAddress = address.StreetAddress,
                    City = address.City
                };

                ViewBag.Cities = new SelectList(GetCities(), address.City);  // Pre-select the existing city
                return View(addressViewModel);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }


        // POST: Addresse/Edit/5
        // POST: Addresse/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AddressUpdateModel addressViewModel)
        {
            if (ModelState.IsValid)
            {
                var address = await db.Addresses.FindAsync(addressViewModel.Id);
                if (address == null)
                {
                    return HttpNotFound();
                }

                // Update address fields except for UserId
                address.StreetAddress = addressViewModel.StreetAddress;
                address.City = addressViewModel.City;

                db.Entry(address).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Cities = new SelectList(GetCities(), addressViewModel.City);  // Repopulate cities in case of an error
            return View(addressViewModel);
        }



        // GET: Addresse/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = await db.Addresses.FindAsync(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // POST: Addresse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Address address = await db.Addresses.FindAsync(id);
            db.Addresses.Remove(address);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
