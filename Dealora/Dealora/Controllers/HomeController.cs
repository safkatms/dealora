using Dealora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Dealora.Models.ViewModel;

namespace Dealora.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Unauthorized()
        {
            return View();
        }
        private HttpClient client;

        public HomeController()
        {
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri(@"http://localhost:9570/api/");
        }


        // GET: Home
        public async Task<ActionResult> Index()
        {
            // Fetch Products and Categories from API
            var response1 = await client.GetAsync("Products");
            var response2 = await client.GetAsync("Categories");

            // Check if both requests were successful
            if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                // Read the content of the responses
                var products = await response1.Content.ReadAsAsync<IEnumerable<Product>>();
                var categories = await response2.Content.ReadAsAsync<IEnumerable<Category>>();

                // Create ViewModel to pass to the View
                var viewModel = new HomeViewModel
                {
                    Products = products,
                    Categories = categories
                };

                // Return the View with the ViewModel
                return View(viewModel);
            }
            else
            {
                // If any of the API calls fail, return an error
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error retrieving data from API.");
            }
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}