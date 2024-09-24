using Dealora.Context;
using Dealora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;


namespace Dealora.Controllers.API
{
    public class ProductsController : ApiController
    {

        private DealoraAppDbContext _dbContext;

        public ProductsController()
        {
            this._dbContext = new DealoraAppDbContext();   
        }

        // GET: Products
        [Route("api/products")]
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            return _dbContext.Products.ToList();
        }

        // GET: api/products/{id}
        [HttpGet]
        [Route("api/products/{id}")]
        public IHttpActionResult GetProduct(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        //Add product
        [Route("api/addproduct")]
        [HttpPost]
        public IHttpActionResult AddProduct(Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(); 

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            return Ok(product);
        }

        //update prodcut by id

        public IHttpActionResult UpdateGames(int id, Product product)
        {
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var data = _dbContext.Products.FirstOrDefault(g => g.Id == id);

            if (data == null)
                return NotFound();

            data.Name = product.Name;
            data.Description= product.Description;
            data.Price = product.Price;
            data.StockQuantity = product.StockQuantity;
            data.IsActive = product.IsActive;
            data.ImageUrl = product.ImageUrl;
            data.DateAdded = product.DateAdded;


            _dbContext.SaveChanges();

            return Ok();
        }
        [Route("api/deletegames/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteProduct(int id) 
        {
            var data = _dbContext.Products.FirstOrDefault(g => g.Id == id);

            //Check product existance
            if (data == null)
                return NotFound(); 

            _dbContext.Products.Remove(data);
            _dbContext.SaveChanges();

            return Ok();
        }
    }
}