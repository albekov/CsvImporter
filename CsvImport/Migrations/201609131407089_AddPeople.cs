using System.Data.Entity.Migrations;

namespace CsvImport.Migrations
{
    public partial class AddPeople : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.People",
                    c => new
                    {
                        Id = c.Int(false, true),
                        FullName = c.String(),
                        BirthDate = c.String(),
                        Email = c.String(),
                        Phone = c.String()
                    })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.People");
        }
    }
}