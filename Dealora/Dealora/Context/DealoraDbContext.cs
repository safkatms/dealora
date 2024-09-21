using Dealora.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace Dealora.Context
{
    public class DealoraDbContext : DbContext
    {
        // Your context has been configured to use a 'DealoraDbContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Dealora.Context.DealoraDbContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'DealoraDbContext' 
        // connection string in the application configuration file.
        public DealoraDbContext()
            : base("name=DealoraDbContext")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Shipping> Shippings { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}