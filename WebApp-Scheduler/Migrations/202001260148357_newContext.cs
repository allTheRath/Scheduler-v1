namespace WebApp_Scheduler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newContext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseWithTimeAllocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        CourseName = c.String(),
                        Topic = c.String(),
                        TimeAllocationHelperId = c.Int(nullable: false),
                        AmountOfTeachingHours = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TimeAllocationHelpers", t => t.TimeAllocationHelperId, cascadeDelete: true)
                .Index(t => t.TimeAllocationHelperId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseWithTimeAllocations", "TimeAllocationHelperId", "dbo.TimeAllocationHelpers");
            DropIndex("dbo.CourseWithTimeAllocations", new[] { "TimeAllocationHelperId" });
            DropTable("dbo.CourseWithTimeAllocations");
        }
    }
}
