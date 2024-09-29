using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
    
    public class AddressesController : ApiController
    {
        private DealoraAppDbContext db = new DealoraAppDbContext();

        // GET: api/Addresses/User/{userId}
        [HttpGet]
        [Authorize(Roles = "Customer")]
        [Route("api/Addresses/{userId}")]
        public IHttpActionResult GetAddressesByUserId(int userId)
        {
            // Get all addresses for the specified user
            var addresses = db.Addresses.Where(a => a.UserId == userId).ToList();

            if (addresses == null || !addresses.Any())
            {
                return NotFound();
            }

            return Ok(addresses);
        }



        // POST: api/Addresses
        [ResponseType(typeof(Address))]
        public async Task<IHttpActionResult> PostAddress(Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Addresses.Add(address);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = address.Id }, address);
        }

    }
}