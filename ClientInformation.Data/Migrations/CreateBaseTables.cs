using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInformation.Data.Migrations
{
    [Migration(202308021713)]
    public class CreateBaseTables : Migration
    {
        public override void Up()
        {

            Create.Table("ClientInformation")
                .WithColumn("Id").AsGuid().WithDefault(SystemMethods.NewGuid).NotNullable().PrimaryKey()
                .WithColumn("FirstName").AsString(50).NotNullable()
                .WithColumn("LastName").AsString(50).NotNullable()
                .WithColumn("Gender").AsString(10).NotNullable()
                .WithColumn("DateOfBirth").AsDate().NotNullable()
                .WithColumn("IdNumber").AsString(13).NotNullable();

            Create.Table("Address")
                .WithColumn("Id").AsGuid().WithDefault(SystemMethods.NewGuid).NotNullable().PrimaryKey()
                .WithColumn("ClientId").AsGuid().ForeignKey("ClientInformation", "Id")
                .WithColumn("Street").AsString(100)
                .WithColumn("City").AsString(50)
                .WithColumn("Province").AsString(50)
                .WithColumn("PostalCode").AsString(10);

            Create.Table("ContactInformation")
                .WithColumn("Id").AsGuid().WithDefault(SystemMethods.NewGuid).NotNullable().PrimaryKey()
                .WithColumn("ClientId").AsGuid().ForeignKey("ClientInformation", "Id")
                .WithColumn("TelePhoneNumber").AsString(15).Nullable()
                .WithColumn("CellPhoneNumber").AsString(15).Nullable()
                .WithColumn("EmailAddress").AsString(50).NotNullable()
                .WithColumn("WorkPhoneNumber").AsString(15).Nullable();
        }

        public override void Down()
        {

        }
    }
}
