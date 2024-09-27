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
        [Route("api/Addresses/{userId}")]
        public IHttpActionResult GetAddressesByUserId(int userId)
        {
            var address = db.Addresses.FirstOrDefault(a => a.UserId == userId);  // Fetching a single address

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }




        // PUT: api/Addresses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAddress(int id, Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != address.Id)
            {
                return BadRequest();
            }

            db.Entry(address).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
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

        // DELETE: api/Addresses/5
        [ResponseType(typeof(Address))]
        public async Task<IHttpActionResult> DeleteAddress(int id)
        {
            Address address = await db.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            db.Addresses.Remove(address);
            await db.SaveChangesAsync();

            return Ok(address);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AddressExists(int id)
        {
            return db.Addresses.Count(e => e.Id == id) > 0;
        }
    }
}