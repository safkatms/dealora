using Dealora.Context;
using Dealora.Models;
using Dealora.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dealora.Controllers
{
    public class OrderController : Controller
    {
        private readonly DealoraAppDbContext _context;

        public OrderController()
        {
            _context = new DealoraAppDbContext();
        }

        // GET: OrderHistory
        public async Task<ActionResult> Index()
        {
            // Check if the user is authenticated
            if (Session["JWTToken"] == null || string.IsNullOrEmpty(Session["JWTToken"].ToString()))
            {
                return RedirectToAction("Login", "User"); // Redirect to login if not authenticated
            }

            // Check user type
            if (Session["Type"]?.ToString() != "Customer")
            {
                return RedirectToAction("Unauthorized", "Home"); // Redirect if user type is not Customer
            }

            var userId = Session["UserId"] != null ? (int)Session["UserId"] : 0;

            // Retrieve orders for the customer
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync();

            var orderHistoryList = orders.Select(o => new OrderHistoryViewModel
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString()
            }).ToList();

            return View(orderHistoryList);
        }

        // GET: OrderHistory/Details/5
        public async Task<ActionResult> Details(int id)
        {
            // Check if the user is authenticated
            if (Session["JWTToken"] == null || string.IsNullOrEmpty(Session["JWTToken"].ToString()))
            {
                return RedirectToAction("Login", "User"); // Redirect to login if not authenticated
            }

            // Check user type
            if (Session["Type"]?.ToString() != "Customer")
            {
                return RedirectToAction("Unauthorized", "Home"); // Redirect if user type is not Customer
            }

            var userId = Session["UserId"] != null ? (int)Session["UserId"] : 0;



            // Retrieve the order details for the specific order
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId); // Ensure the order belongs to the user

            if (order == null)
            {
                return HttpNotFound();
            }

            // Retrieve the order items for the order
            var orderItems = await _context.OrderItems
                .Include(oi => oi.Product) // Include product details
                .Where(oi => oi.OrderId == order.Id)
                .ToListAsync();

            // Map to ViewModel
            var viewModel = new OrderHistoryViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                OrderItems = orderItems.Select(oi => new OrderItemViewModel
                {
                    ProductName = oi.Product.Name, // Assuming Product has a Name property
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase
                }).ToList()
            };

            return View(viewModel);
        }


        public async Task<ActionResult> SellerOrders()
        {
            if (Session["JWTToken"] == null || string.IsNullOrEmpty(Session["JWTToken"].ToString()))
            {
                return RedirectToAction("Login", "User"); // Redirect to login if not authenticated
            }

            if (Session["Type"]?.ToString() != "Seller")
            {
                return RedirectToAction("Unauthorized", "Home"); // Redirect if user type is not Customer
            }
            var sellerId = Session["UserId"] != null ? (int)Session["UserId"] : 0;

            

            // Retrieve all order items for the seller along with customer info
            var sellerOrders = await _context.OrderItems
                .Include(oi => oi.Order) // Include the Order details
                .Include(oi => oi.Product) // Include the Product details
                .Include(oi => oi.Order.User) // Include Customer information
                .Where(oi => oi.Product.UserId == sellerId) // Filter by seller ID
                .Select(oi => new SellerOrderViewModel
                {
                    OrderId = oi.OrderId,
                    OrderItemId = oi.Id,
                    OrderDate = oi.Order.OrderDate,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase,
                    TotalAmount = oi.Quantity * oi.PriceAtPurchase, // Calculate total for the item
                    CustomerName = oi.Order.User.FirstName + " " + oi.Order.User.LastName, // Concatenate first and last name
                    CustomerEmail = oi.Order.User.Email,
                    CurrentStatus = oi.Order.Status.ToString(), // Use oi.Status instead of oi.Order.Status
                    CustomerPhoneNumber = oi.Order.User.PhoneNumber
                })
                .ToListAsync();

            return View(sellerOrders);
        }


        public async Task<ActionResult> ChangeOrderItemStatus(int orderId)
        {
            if (Session["JWTToken"] == null || string.IsNullOrEmpty(Session["JWTToken"].ToString()))
            {
                return RedirectToAction("Login", "User"); 
            }

            if (Session["Type"]?.ToString() != "Seller")
            {
                return RedirectToAction("Unauthorized", "Home"); 
            }
            
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order) // Include Order details
                .Include(oi => oi.Product) // Include Product details
                .FirstOrDefaultAsync(oi => oi.Id == orderId);

            if (orderItem == null)
            {
                return HttpNotFound();
            }

            // Create and return the view model for your new view
            var viewModel = new SellerOrderViewModel
            {
                OrderId = orderItem.Order.Id,
                OrderItemId = orderId,
                CurrentStatus = orderItem.Status.ToString(),
                ProductName = orderItem.Product.Name,
                Quantity = orderItem.Quantity,
                PriceAtPurchase = orderItem.PriceAtPurchase,
                TotalAmount = orderItem.Quantity * orderItem.PriceAtPurchase,
                CustomerName = orderItem.Order.User.FirstName + " " + orderItem.Order.User.LastName,
                CustomerEmail = orderItem.Order.User.Email,
                CustomerPhoneNumber = orderItem.Order.User.PhoneNumber,
                // Add other properties as needed
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateOrderItemStatus(int orderItemId, OrderItemStatus newStatus)
        {
            // Fetch the order item by ID
            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId);

            if (orderItem == null)
            {
                return HttpNotFound();
            }

            // Get the associated Product ID
            var productId = orderItem.ProductId; // Ensure this property exists in your OrderItem model

            // Update the order item's status
            orderItem.Status = newStatus;

            // Save the change to the order item
            await _context.SaveChangesAsync();

            // Get the associated Order ID
            var orderId = orderItem.OrderId;

            // Fetch all OrderItems associated with the same OrderId
            var allOrderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            // Check if all OrderItems have the same status
            bool allStatusesSame = allOrderItems.All(oi => oi.Status == newStatus);

            if (allStatusesSame)
            {
                // Fetch the order from the Orders table
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

                if (order != null)
                {
                    
                    order.Status = (OrderStatus)newStatus;

                    
                    if (newStatus == OrderItemStatus.Cancelled)
                    {
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

                        if (product != null)
                        {
                            product.StockQuantity += orderItem.Quantity; 
                        }
                    }

                   
                    await _context.SaveChangesAsync();
                }
            }

            
            TempData["SuccessMessage"] = "Order item status updated successfully.";
            return RedirectToAction("SellerOrders", "Order");
        }

    }
}
