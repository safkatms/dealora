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
        [HttpGet]
        [Route("api/products")]
        public IEnumerable<Product> GetProducts()
        {
            return _dbContext.Products.ToList();
        }

        //Get: Returnproducts spec user
        [HttpGet]
        [Route("api/products/userId/{userId}")]
        public IEnumerable<Product> GetProductsByUserId(int userId)
        {
            return _dbContext.Products.Where(p => p.UserId == userId).ToList();
        }
        // GET: api/products/{id}
        [HttpGet]
        [Route("api/products/{id}")]
        public IHttpActionResult GetProduct(int id)
        {
            //validation
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }
        //Get: by category id
        [HttpGet]
        [Route("api/products/category/{categoryId}")]
        public IEnumerable<Product> GetProductsByCategoryId(int categoryId)
        {
            return _dbContext.Products.Where(p => p.CategoryId == categoryId).ToList();
        }

        //Add product
        [Route("api/addproduct")]
        [HttpPost]
        public IHttpActionResult AddProduct(Product product)
        {
            //validation
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (product.StockQuantity < 0)
            {
                return BadRequest("Stock quantity cannot be negative.");
            }

            if (product.Price < 0)
            {
                return BadRequest("Price cannot be negative.");
            }
        
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            return Ok(product);
        }

        //update prodcut by id
        [Route("api/updateproduct/{id}")]
        [HttpPut]
        public IHttpActionResult UpdateProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (product.StockQuantity < 0)
            {
                return BadRequest("Stock quantity cannot be negative.");
            }

            if (product.Price < 0)
            {
                return BadRequest("Price cannot be negative.");
            }

            var existingProduct = _dbContext.Products.FirstOrDefault(g => g.Id == id);

            if (existingProduct == null)
                return NotFound();

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.IsActive = product.IsActive;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.DateAdded = product.DateAdded;


            _dbContext.SaveChanges();

            return Ok(product);
        }
        [Route("api/deleteproduct/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteProduct(int id) 
        {
            var existingProduct = _dbContext.Products.FirstOrDefault(g => g.Id == id);

            //Check product existance
            if (existingProduct == null)
                return NotFound(); 

            _dbContext.Products.Remove(existingProduct);
            _dbContext.SaveChanges();

            return Ok(existingProduct);
        }
    }
}