namespace Tdlr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class users : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ObjectId = c.String(nullable: false, maxLength: 128),
                        assignmentStatus = c.String(),
                        City = c.String(),
                        CompanyName = c.String(),
                        JobTitle = c.String(),
                        Country = c.String(),
                        DisplayName = c.String(),
                        State = c.String(),
                        GivenName = c.String(),
                        Mail = c.String(),
                        MailNickname = c.String(),
                        ObjectType = c.String(),
                        TelephoneNumber = c.String(),
                        Surname = c.String(),
                        UsageLocation = c.String(),
                        UserPrincipalName = c.String(),
                    })
                .PrimaryKey(t => t.ObjectId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
