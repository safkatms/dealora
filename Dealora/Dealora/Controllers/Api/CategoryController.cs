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

        // GET: Products
        [HttpGet]
        [Route("api/Categories")]
        public IEnumerable<Category> GetProducts()
        {
            return _dbContext.Categories.ToList();
        }
    }
}