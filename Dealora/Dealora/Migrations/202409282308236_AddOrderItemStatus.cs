namespace Dealora.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderItemStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItems", "Status");
        }
    }
}
