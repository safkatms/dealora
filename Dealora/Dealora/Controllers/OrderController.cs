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
            // Get seller ID from session
            var sellerId = Session["UserId"] != null ? (int)Session["UserId"] : 0;

            // Check if seller ID is valid
            if (sellerId == 0)
            {
                return RedirectToAction("Login", "User"); // Redirect if not logged in
            }

            // Retrieve all order items for the seller along with customer info
            var sellerOrders = await _context.OrderItems
                .Include(oi => oi.Order) // Include the Order details
                .Include(oi => oi.Product) // Include the Product details
                .Include(oi => oi.Order.User) // Include Customer information
                .Where(oi => oi.Product.UserId == sellerId) // Filter by seller ID
                .Select(oi => new SellerOrderViewModel
                {
                    OrderId = oi.OrderId,
                    OrderDate = oi.Order.OrderDate,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase,
                    TotalAmount = oi.Quantity * oi.PriceAtPurchase, // Calculate total for the item
                    CustomerName = oi.Order.User.FirstName + " " + oi.Order.User.LastName, // Concatenate first and last name
                    CustomerEmail = oi.Order.User.Email,
                    CurrentStatus = oi.Order.Status.ToString(),
                    CustomerPhoneNumber = oi.Order.User.PhoneNumber
                })
                .ToListAsync();

            // Populate ViewBag with OrderItemStatus enum values
            ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(OrderItemStatus)).Cast<OrderItemStatus>().Select(e => new
            {
                Value = e.ToString(),
                Text = e.ToString() // Customize this if you want different display names
            }), "Value", "Text");

            return View(sellerOrders);
        }


        [HttpPost]
        public async Task<ActionResult> UpdateOrderItemStatus(int orderItemId, string newStatus)
        {
            // Get the order item from the database
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order) // Include the Order details
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId);

            if (orderItem == null)
            {
                return HttpNotFound();
            }

            // Convert the new status string to OrderItemStatus enum
            if (Enum.TryParse<OrderItemStatus>(newStatus, out var orderItemStatus))
            {
                // Update the order item's status
                orderItem.Status = orderItemStatus;
                _context.Entry(orderItem).State = EntityState.Modified;

                // Check if all order items in the order have the same status
                var allOrderItems = await _context.OrderItems
                    .Where(oi => oi.OrderId == orderItem.OrderId)
                    .ToListAsync();

                // Check if all order items have the same status
                if (allOrderItems.All(oi => oi.Status == orderItemStatus))
                {
                    // Set the order status based on the order item status
                    OrderStatus orderStatus;

                    // Here we assume the logic is that if all order items are delivered, the order is delivered.
                    switch (orderItemStatus)
                    {
                        case OrderItemStatus.Pending:
                            orderStatus = OrderStatus.Pending;
                            break;
                        case OrderItemStatus.Shipped:
                            orderStatus = OrderStatus.Shipped;
                            break;
                        case OrderItemStatus.Delivered:
                            orderStatus = OrderStatus.Delivered;
                            break;
                        case OrderItemStatus.Cancelled:
                            orderStatus = OrderStatus.Cancelled;
                            break;
                        default:
                            orderStatus = OrderStatus.Pending; // Default case
                            break;
                    }

                    // Update the order status
                    orderItem.Order.Status = orderStatus; // Set the Order status
                    _context.Entry(orderItem.Order).State = EntityState.Modified; // Mark the order as modified
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction("Index","Home"); // Redirect to the seller orders page or wherever appropriate
            }
            else
            {
                // Handle invalid status case (optional)
                ModelState.AddModelError("", "Invalid order status.");
                return RedirectToAction("SellerOrders");
            }
        }




    }
}
