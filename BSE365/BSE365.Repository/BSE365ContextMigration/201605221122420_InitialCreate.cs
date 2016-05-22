namespace BSE365.Repository.BSE365ContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Configs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(maxLength: 50),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Key, unique: true, name: "Unq_LoginName");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Configs", "Unq_LoginName");
            DropTable("dbo.Configs");
        }
    }
}
