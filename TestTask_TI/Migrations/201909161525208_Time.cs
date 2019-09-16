namespace TestTask_TI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Time : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "CreatedTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "CreatedTime");
        }
    }
}
