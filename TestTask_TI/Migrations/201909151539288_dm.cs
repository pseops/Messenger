namespace TestTask_TI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dm : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderId = c.String(),
                        ReceiverId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        CreatorId = c.String(),
                        Name = c.String(),
                        Chats_Id = c.Int(nullable: false),
                        Chats_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chats", t => t.Chats_Id1)
                .Index(t => t.Chats_Id1);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "Chats_Id1", "dbo.Chats");
            DropIndex("dbo.Messages", new[] { "Chats_Id1" });
            DropTable("dbo.Messages");
            DropTable("dbo.Chats");
        }
    }
}
