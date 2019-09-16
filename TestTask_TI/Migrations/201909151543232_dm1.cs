namespace TestTask_TI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dm1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Messages", "Chats_Id1", "dbo.Chats");
            DropIndex("dbo.Messages", new[] { "Chats_Id1" });
            DropColumn("dbo.Messages", "Chats_Id1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "Chats_Id1", c => c.Int());
            CreateIndex("dbo.Messages", "Chats_Id1");
            AddForeignKey("dbo.Messages", "Chats_Id1", "dbo.Chats", "Id");
        }
    }
}
