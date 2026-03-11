namespace Prescriptions.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddUniqueIndexToPrescriptionCode : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Prescriptions", "Code", unique: true, name: "IX_Prescriptions_Code");
        }

        public override void Down()
        {
            DropIndex("dbo.Prescriptions", "IX_Prescriptions_Code");
        }
    }
}