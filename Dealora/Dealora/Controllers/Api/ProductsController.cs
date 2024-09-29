using Dealora.Context;
using Dealora.Models;
using Dealora.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        [Authorize(Roles = "Seller")]
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
            /*existingProduct.ImageUrl = product.ImageUrl;*/
            existingProduct.CategoryId = product.CategoryId;


            _dbContext.SaveChanges();

            return Ok(product);
        }

        //delete product
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

        [HttpGet]
        [Route("api/seller/orders/{userId}")]
        public IEnumerable<OrderDeatilsViewModel> GetOrdersBySeller(int userId)
        {
            // Get the products that belong to the seller
            var productIds = _dbContext.Products
                .Where(p => p.UserId == userId)
                .Select(p => p.Id)
                .ToList();

            // Get the order IDs for those products
            var orderIds = _dbContext.OrderItems
                .Where(oi => productIds.Contains(oi.ProductId))
                .Select(oi => oi.OrderId)
                .Distinct()
                .ToList();

            // Retrieve the orders that match those IDs, including order items
            var orders = _dbContext.Orders
                .Where(o => orderIds.Contains(o.Id))
                .ToList();

            // Create a list of OrderDetailsViewModel
            var orderDetailsList = orders.Select(order => new OrderDeatilsViewModel
            {
                Order = order,
                OrderItems = _dbContext.OrderItems
                    .Where(oi => oi.OrderId == order.Id)
                    .ToList() // Get associated order items for theorder
            }).ToList();

            return orderDetailsList; 
        }

        // GET: api/seller/dashboard/{userId}
        [HttpGet]
        [Route("api/seller/dashboard/{userId}")]
        public IHttpActionResult GetSellerDashboard(int userId)
        {
            // seller prdct
            var products = _dbContext.Products
                .Where(p => p.UserId == userId)
                .ToList();

            // Group products by category and get the count
            var dashboardData = products
                .GroupBy(p => p.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    ProductCount = g.Count(),
                    CategoryName = _dbContext.Categories.FirstOrDefault(c => c.Id == g.Key)?.Name 
                })
                .ToList();

            return Ok(dashboardData);
        }



    }

}
