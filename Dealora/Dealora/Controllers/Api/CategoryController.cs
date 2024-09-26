using Dealora.Context;
using Dealora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace Dealora.Controllers.API
{
    public class CategoryController : ApiController
    {
        private DealoraAppDbContext _dbContext;

        public CategoryController()
        {
            this._dbContext = new DealoraAppDbContext();
        }

        // GET: Category
        [HttpGet]
        [Route("api/Categories")]
        public IEnumerable<Category> GetCategories()
        {
            return _dbContext.Categories.ToList();
        }

        //Post : Category
        [Route("api/addcategories")]
        [HttpPost]
        public IHttpActionResult AddCategory(Category category)
        {
            //validation
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();

            return Ok(category);
        }
    }
}