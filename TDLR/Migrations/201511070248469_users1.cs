namespace Tdlr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class users1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tenants",
                c => new
                    {
                        TenantId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.TenantId);
            
            AddColumn("dbo.Users", "Tenant_TenantId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Users", "Tenant_TenantId");
            AddForeignKey("dbo.Users", "Tenant_TenantId", "dbo.Tenants", "TenantId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Tenant_TenantId", "dbo.Tenants");
            DropIndex("dbo.Users", new[] { "Tenant_TenantId" });
            DropColumn("dbo.Users", "Tenant_TenantId");
            DropTable("dbo.Tenants");
        }
    }
}
