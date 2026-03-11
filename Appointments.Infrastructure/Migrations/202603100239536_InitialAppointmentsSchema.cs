namespace Appointments.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialAppointmentsSchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DoctorId = c.Guid(nullable: false),
                        PatientId = c.Guid(nullable: false),
                        ScheduledAt = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Appointments");
        }
    }
}
