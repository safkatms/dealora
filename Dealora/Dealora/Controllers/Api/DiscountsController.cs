using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Dealora.Context;
using Dealora.Models;

namespace Dealora.Controllers.API
{
    public class DiscountsController : ApiController
    {
        private readonly DealoraAppDbContext db = new DealoraAppDbContext();

        // GET: api/Discounts
        [Authorize(Roles = "Admin")]
        public IQueryable<Discount> GetDiscounts()
        {
            return db.Discounts;
        }

        // GET: api/Discounts/5
        [ResponseType(typeof(Discount))]
        public async Task<IHttpActionResult> GetDiscount(int id)
        {
            var discount = await db.Discounts.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }

            return Ok(discount);
        }

        // POST: api/Discounts
        [ResponseType(typeof(Discount))]
        public async Task<IHttpActionResult> PostDiscount(Discount discount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Discounts.Add(discount);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = discount.Id }, discount);
        }

        // Dispose DbContext
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
