namespace Prescriptions.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialPrescriptionsSchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prescriptions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(nullable: false, maxLength: 100),
                        PatientId = c.Guid(nullable: false),
                        IssuedAt = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Prescriptions");
        }
    }
}
