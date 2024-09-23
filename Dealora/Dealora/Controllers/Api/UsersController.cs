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
using System.Security.Claims;
using System.Text;
using System.Web.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt; // For JwtSecurityToken, JwtSecurityTokenHandler
using Microsoft.IdentityModel.Tokens;  // For SymmetricSecurityKey, SigningCredentials, SecurityAlgorithms
using System.Security.Claims;          // For Claim, ClaimTypes
using System.Text;                     // For Encoding


namespace Dealora.Controllers
{
    public class UsersController : ApiController
    {

        //public string HashPassword(string password)
        //{
        //    var passwordHasher = new PasswordHasher<object>();
        //    return passwordHasher.HashPassword(null, password);
        //}

        //public bool VerifyPassword(string hashedPassword, string enteredPassword)
        //{
        //    var passwordHasher = new PasswordHasher<object>();
        //    var result = passwordHasher.VerifyHashedPassword(null, hashedPassword, enteredPassword);

        //    return result == PasswordVerificationResult.Success;
        //}



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
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the email is already registered
            if (db.Users.Any(u => u.Email == user.Email))
            {
                return Conflict();
            }


            //// Hash the password
            //user.Password = HashPassword(user.Password); 

            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("UserSignup", new { id = user.Id }, user);
        }


        // POST: api/Users/Login
        [HttpPost]
        [Route("api/Users/Login")]
        public IHttpActionResult Login(User loginModel)
        {
            if (loginModel == null || string.IsNullOrEmpty(loginModel.Email) || string.IsNullOrEmpty(loginModel.Password))
            {
                return BadRequest("Invalid user credentials.");
            }

            // Retrieve the user from the database based on the email
            var user = db.Users.FirstOrDefault(x => x.Email == loginModel.Email && x.Password == loginModel.Password);

            if (user == null)
            {
                return Unauthorized();  // Invalid email or password
            }

            // Generate a JWT Token
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token = token,
                userDetails = new
                {
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    Role = user.Role.ToString()
                }
            });
        }

        // Method to generate JWT token
        // Method to generate JWT token
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your_Secret_Key_Must_Be_32_Chars_Long"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create claims for the token
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString()), // Add role claim
        new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Token ID
    };

            // Generate the token
            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
