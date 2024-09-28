namespace Dealora.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCategoryTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "CategoryImageUrl", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "CategoryImageUrl");
        }
    }
}
