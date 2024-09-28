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


        // GET: User
        public async Task<ActionResult> Index()
        {

                var response = await client.GetAsync("products");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<IEnumerable<Product>>();
                    return View(data);
                }
                else
                {
                    return new HttpStatusCodeResult(response.StatusCode, "Error retrieving data from API.");
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