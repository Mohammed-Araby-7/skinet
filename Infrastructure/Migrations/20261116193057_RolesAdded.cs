
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class RolesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM dbo.AspNetRoles
                    WHERE NormalizedName = N'ADMIN'
                )
                BEGIN
                    INSERT INTO dbo.AspNetRoles
                        (Id, ConcurrencyStamp, Name, NormalizedName)
                    VALUES
                        (N'8c8af3e8-a5a9-4d11-87a1-f0e78fb3fb9d',
                        NULL, N'Admin', N'ADMIN');
                END;

                IF NOT EXISTS (
                    SELECT 1 FROM dbo.AspNetRoles
                    WHERE NormalizedName = N'CUSTOMER'
                )
                BEGIN
                    INSERT INTO dbo.AspNetRoles
                        (Id, ConcurrencyStamp, Name, NormalizedName)
                    VALUES
                        (N'ee96ca96-94fe-4755-9aa1-80317c20c9f7',
                        NULL, N'Customer', N'CUSTOMER');
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c8af3e8-a5a9-4d11-87a1-f0e78fb3fb9d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ee96ca96-94fe-4755-9aa1-80317c20c9f7");
        }
    }
}
