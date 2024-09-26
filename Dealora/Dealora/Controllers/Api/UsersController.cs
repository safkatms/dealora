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

        [Authorize(Roles ="Admin")]
        // GET: api/Users
        public IEnumerable<User> GetUsers()
        {
            return db.Users.Where(u => u.Role != UserRole.Admin).ToList();
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

            if (db.Users.Any(u => u.Email == user.Email))
            {
                return Conflict();
            }

            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("UserSignup", new { id = user.Id }, user);
        }

        


        //view profile
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

        //update profile
        [Route("api/Users/Update/{id}")]
        [HttpPut]
        public IHttpActionResult UpdateUsers(int id, [FromBody] UserUpdateModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 

            var data = db.Users.FirstOrDefault(u => u.Id == id);

            if (data == null)
                return NotFound();

            // Update the fields as necessary
            data.FirstName = user.FirstName;
            data.LastName = user.LastName;
            data.PhoneNumber = user.PhoneNumber;


            db.SaveChanges();

            return Ok();
        }

        [HttpPut]
        [Route("api/Users/ChangePassword/{id}")]
        public IHttpActionResult ChangePassword(int id, [FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = db.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound(); 
            }

            if (user.Password != model.CurrentPassword)
            {
                return BadRequest("The current password is incorrect.");
            }


            user.Password = model.NewPassword;
            db.SaveChanges();
            return Ok("Password changed successfully.");
        }




        [HttpPost]
        [Route("api/Users/Login")]
        public IHttpActionResult Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
