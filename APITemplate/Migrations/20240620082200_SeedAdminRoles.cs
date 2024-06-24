using APITemplate.DBModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APITemplate.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData("AspNetRoles",
                columns: new string[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] {Guid.NewGuid().ToString(), "User", "User".ToUpper(), Guid.NewGuid().ToString()});

            var adminRoleID = Guid.NewGuid().ToString();
            migrationBuilder.InsertData("AspNetRoles",
                columns: new string[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { adminRoleID, "Admin", "Admin".ToUpper(), Guid.NewGuid().ToString() });

            // Add Admin User
            var adminID = Guid.NewGuid().ToString();
            var admin = new ApplicationUser()
            {
                Id = adminID,
                FullName = "Test",
                UserName = "Test",
                NormalizedUserName = "Test".ToUpper(),
                Email = "test@test.com",
                NormalizedEmail = "test@test.com".ToUpper(),
                EmailConfirmed = true,
                LockoutEnabled = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var hashedPassword = passwordHasher.HashPassword(admin, "test1234.");
            admin.PasswordHash = hashedPassword;
            migrationBuilder.InsertData("AspNetUsers",
                columns: new string[] 
                {
                    "Id",
                    "FullName",
                    "UserName",
                    "NormalizedUserName",
                    "Email",
                    "NormalizedEmail",
                    "EmailConfirmed",
                    "PasswordHash",
                    "SecurityStamp",
                    "PhoneNumberConfirmed",
                    "TwoFactorEnabled",
                    "LockoutEnabled",
                    "AccessFailedCount" 
                },
                values: new object[] {
                    admin.Id,
                    admin.FullName,
                    admin.UserName,
                    admin.NormalizedUserName,
                    admin.Email,
                    admin.NormalizedEmail,
                    admin.EmailConfirmed,
                    admin.PasswordHash,
                    admin.SecurityStamp,
                    admin.PhoneNumberConfirmed,
                    admin.TwoFactorEnabled,
                    admin.LockoutEnabled,
                    admin.AccessFailedCount
                });

            migrationBuilder.InsertData("AspNetUserRoles",
                columns: new string[] { "UserId", "RoleId" },
                values: new string[] {adminID, adminRoleID});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete From [AspNetRoles]");
        }
    }
}
