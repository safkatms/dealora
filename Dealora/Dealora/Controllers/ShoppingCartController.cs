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
            if (Session["Type"].ToString() != "Customer")
            {
                return RedirectToAction("Unauthorized", "Home");
            }
            var userId = Session["UserId"] != null ? (int)Session["UserId"] : 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "User");
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
            // Get user ID from session
            var userId = Session["UserId"] != null ? (int)Session["UserId"] : 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "User");
            }

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

            if (userId == 0)
            {
                return RedirectToAction("Login", "User");
            }

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
