namespace Tdlr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notenants : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "Tenant_TenantId", "dbo.Tenants");
            DropIndex("dbo.Users", new[] { "Tenant_TenantId" });
            AddColumn("dbo.Users", "TenantId", c => c.String());
            DropColumn("dbo.Users", "Tenant_TenantId");
            DropTable("dbo.Tenants");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Tenants",
                c => new
                    {
                        TenantId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.TenantId);
            
            AddColumn("dbo.Users", "Tenant_TenantId", c => c.String(maxLength: 128));
            DropColumn("dbo.Users", "TenantId");
            CreateIndex("dbo.Users", "Tenant_TenantId");
            AddForeignKey("dbo.Users", "Tenant_TenantId", "dbo.Tenants", "TenantId");
        }
    }
}
