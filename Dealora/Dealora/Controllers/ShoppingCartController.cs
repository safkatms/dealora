using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dealora.Context;
using Dealora.Models;
using Dealora.Models.ViewModel;
using Dealora.ViewModels;

namespace Dealora.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly DealoraAppDbContext _context;

        public ShoppingCartController()
        {
            _context = new DealoraAppDbContext();
        }
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

            if (userId == 0)
            {
                return RedirectToAction("Login", "User"); // Redirect to login if user ID is invalid
            }

            // Retrieve the shopping cart for the user
            var shoppingCart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(sc => sc.UserId == userId);

            if (shoppingCart == null)
            {
                return View(new ShoppingCartViewModel
                {
                    CartItems = new List<CartItem>(), // Empty cart
                    TotalAmount = 0
                });
            }

            // Retrieve CartItems for the shopping cart
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product) // Include Product details
                .Where(ci => ci.ShoppingCartId == shoppingCart.Id)
                .ToListAsync();

            // Calculate total amount
            var totalAmount = cartItems.Sum(ci => ci.Quantity * ci.Product.Price);

            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cartItems,
                TotalAmount = totalAmount
            };

            return View(viewModel);
        }

        // GET: ShoppingCart/AddToCart
        [HttpGet]
        public async Task<ActionResult> AddToCart(int productId, int quantity = 1) // Default quantity set to 1
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

           



            // Find the shopping cart for the user
            var shoppingCart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(sc => sc.UserId == userId);

            // If the shopping cart doesn't exist, create a new one
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart { UserId = userId };
                _context.ShoppingCarts.Add(shoppingCart);
                await _context.SaveChangesAsync();
            }

            // Check if the item is already in the cart
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ShoppingCartId == shoppingCart.Id && ci.ProductId == productId);

            if (existingCartItem != null)
            {
                // Item already exists, update quantity
                existingCartItem.Quantity += quantity;
                _context.Entry(existingCartItem).State = EntityState.Modified;
            }
            else
            {
                // Create a new cart item
                var newCartItem = new CartItem
                {
                    ShoppingCartId = shoppingCart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(newCartItem);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the shopping cart index page
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> UpdateQuantity(int id, int quantity)
        {
            
            // Validate quantity
            if (quantity < 1)
            {
                return RedirectToAction("Index"); // Handle invalid quantity
            }

            // Get user ID from session
            var userId = Session["UserId"] != null ? (int)Session["UserId"] : 0;


            // Find the cart item
            var cartItem = await _context.CartItems.Include(ci => ci.Product).FirstOrDefaultAsync(ci => ci.Id == id);

            if (cartItem != null && cartItem.ShoppingCart.UserId == userId)
            {
                // Check if the requested quantity is available
                if (quantity > cartItem.Product.StockQuantity)
                {
                    // Optionally, you could store a message to show in the view
                    TempData["ErrorMessage"] = "Requested quantity exceeds available stock.";
                    return RedirectToAction("Index"); // Redirect back to the shopping cart
                }

                cartItem.Quantity = quantity; // Update the quantity
                await _context.SaveChangesAsync(); // Save changes
            }

            return RedirectToAction("Index"); // Redirect back to the shopping cart
        }



        // GET: ShoppingCart/RemoveFromCart/5
        public async Task<ActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }



        // GET: Checkout
        [HttpGet]
        public async Task<ActionResult> Checkout()
        {
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

           
            // Retrieve the user's shopping cart and addresses
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.ShoppingCart.UserId == userId)
                .ToListAsync();

            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var viewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                Addresses = addresses,
                TotalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity),
            };

            return View(viewModel);
        }

        // POST: Checkout
        [HttpPost]
        public async Task<ActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = Session["UserId"] != null ? (int)Session["UserId"] : 0;

            

            // Retrieve the user's cart items again in case of changes during checkout
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.ShoppingCart.UserId == userId)
                .ToListAsync();

            // Calculate the total amount based on cart items
            model.TotalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

            // Apply discount if provided
            if (!string.IsNullOrEmpty(model.DiscountCode))
            {
                // Retrieve the discount from the database
                var discount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.Code == model.DiscountCode && d.IsActive);

                // Check if the discount is expired
                if (discount != null && discount.ExpiryDate < DateTime.Now)
                {
                    discount = null;  // Treat as expired
                }

                if (discount != null)
                {
                    // Apply discount to the total amount
                    model.TotalAmount -= model.TotalAmount * (discount.DiscountPercentage / 100);
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid or expired discount code.";
                    return RedirectToAction("Checkout");
                }
            }

            // Create the order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = model.TotalAmount,
                Status = OrderStatus.Pending,
                AddressId = model.AddressId,
                PaymentMethod = model.PaymentMethod,
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Add order items
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.Product.Price
                };
                _context.OrderItems.Add(orderItem);
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;   // Update the product in the database
                }
            }

            // Remove cart items after order is placed
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }



        // GET: Order Confirmation
        public async Task<ActionResult> OrderConfirmation(int orderId)
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

            

            // Retrieve the order and check if it belongs to the current user
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return RedirectToAction("Unauthorized", "Home"); // Redirect if order does not belong to the user
            }

            return View(order);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
