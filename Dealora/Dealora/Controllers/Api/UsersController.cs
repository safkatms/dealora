using Dealora.Context;
using Dealora.Models;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace Dealora.Controllers
{
    public class UsersController : ApiController
    {

        public string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<object>();
            // Generate the hashed password
            return passwordHasher.HashPassword(null, password);
        }

        public bool VerifyPassword(string hashedPassword, string enteredPassword)
        {
            var passwordHasher = new PasswordHasher<object>();
            // Compare the entered password with the stored hashed password
            var result = passwordHasher.VerifyHashedPassword(null, hashedPassword, enteredPassword);

            return result == PasswordVerificationResult.Success;
        }



        private DealoraDbContext db = new DealoraDbContext();

        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/Users/Signup
        [HttpPost]
        [Route("api/Users/Signup", Name = "UserSignup")]
        [ResponseType(typeof(User))]
        public IHttpActionResult SignUp(User user)
        {
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the email is already registered
            if (db.Users.Any(u => u.Email == user.Email))
            {
                return Conflict(); // Email already exists
            }


            //// Hash the password
            //user.Password = HashPassword(user.Password); // Implement this

            // Add the user to the database
            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("UserSignup", new { id = user.Id }, user);
        }


        // POST: api/Users/Login
        [HttpPost]
        [Route("api/Users/Login")]
        public IHttpActionResult Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the user exists and password matches
            var user = db.Users.FirstOrDefault(u => u.Email == loginModel.Email);
            if (user == null || !VerifyPassword(loginModel.Password, user.Password)) // Implement VerifyPassword
            {
                return Unauthorized(); // Invalid credentials
            }

            // If valid credentials, generate token or session
            // Example: return Ok(new { token = GenerateToken(user), User = user });
            return Ok(new { message = "Login successful", user });
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }

    }

    // Model to handle login request
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
