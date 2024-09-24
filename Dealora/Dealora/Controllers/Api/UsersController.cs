using Dealora.Context;
using Dealora.Models;
using Dealora.Models.ViewModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;

namespace Dealora.Controllers.API
{
    public class UsersController : ApiController
    {
        private DealoraAppDbContext db = new DealoraAppDbContext();


        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
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
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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
