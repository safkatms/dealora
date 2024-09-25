using Dealora.Context;
using Dealora.Models;
using Dealora.Models.ViewModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Dealora.Controllers.API
{
    public class UsersController : ApiController
    {
        private DealoraAppDbContext db = new DealoraAppDbContext();

        [Authorize(Roles ="Customer")]
        // GET: api/Users
        public IEnumerable<User> GetUsers()
        {
            return db.Users.ToList();
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

            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("UserSignup", new { id = user.Id }, user);
        }

        [Route("api/Users/Update/{id}")]
        [HttpPut]
        public IHttpActionResult UpdateUsers(int id, User user)
        {
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var data = db.Users.FirstOrDefault(u => u.Id == id);

            if (data == null)
                return NotFound();

            data.FirstName = user.FirstName;
            data.LastName = user.LastName;
            data.PhoneNumber = user.PhoneNumber;

            db.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("api/Users/{id:int}")]
        public IHttpActionResult GetUserProfile(int id)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPatch]
        [Route("api/Users/ChangePassword/{id}")]
        public IHttpActionResult ChangePassword(int id, [FromBody] ChangePasswordViewModel model)
        {
            // Check if the provided model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user in the database by their ID
            var user = db.Users.FirstOrDefault(u => u.Id == id);

            // Check if the user exists
            if (user == null)
            {
                return NotFound(); // Return 404 if user is not found
            }

            // Validate the current password (Assuming password is stored as plain text, otherwise use hashing)
            if (user.Password != model.CurrentPassword)
            {
                return BadRequest("The current password is incorrect.");
            }

            // Validate if new password and confirmation password match
            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest("The new password and confirmation password do not match.");
            }

            // Update the password to the new one
            user.Password = model.NewPassword;

            try
            {
                // Save changes to the database
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Capture validation errors from the database
                var validationErrors = new List<string>();
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        validationErrors.Add($"Property: {error.PropertyName} Error: {error.ErrorMessage}");
                    }
                }

                // Return a BadRequest with detailed validation error messages
                return BadRequest(string.Join("; ", validationErrors));
            }

            // Return success message if password change is successful
            return Ok("Password changed successfully.");
        }




        [HttpPost]
        [Route("api/Users/Login")]
        public IHttpActionResult Login([FromBody] LoginModel model)
        {
            var user = db.Users.SingleOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null) return Unauthorized();

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes("your-very-long-secret-key-of-at-least-32-characters");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(ClaimTypes.Role, user.Role.ToString()), // Store the user's role
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()) // Convert integer User.Id to string
            };

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
