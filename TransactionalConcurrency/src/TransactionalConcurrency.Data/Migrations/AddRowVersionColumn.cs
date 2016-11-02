using FluentMigrator;

namespace TransactionalConcurrency.Data.Migrations
{
    [Migration(2)]
    public class AddRowVersionColumn : Migration
    {
        public override void Up()
        {
            Alter.Table("Video")
                .AddColumn("RowVersion").AsCustom("rowversion").Nullable();
        }

        public override void Down()
        {
            Delete.Column("RowVersion").FromTable("Video");
        }
    }
}