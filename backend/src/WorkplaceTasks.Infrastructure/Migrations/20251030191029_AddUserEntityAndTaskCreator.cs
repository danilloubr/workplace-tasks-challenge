using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WorkplaceTasks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEntityAndTaskCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { new Guid("5fee76f9-1743-42e3-bdbc-29cf76f02dc1"), new DateTime(2025, 10, 30, 19, 10, 29, 725, DateTimeKind.Utc).AddTicks(5010), "member@example.com", "$2a$11$CIfEGeCFoZjHm0mkSI7uzOcxAOPUpoABbbSl7hGh/7CboBBM4ZNb.", 2 },
                    { new Guid("be46fcc6-d556-41ee-8034-fc454dcea648"), new DateTime(2025, 10, 30, 19, 10, 29, 627, DateTimeKind.Utc).AddTicks(6050), "manager@example.com", "$2a$11$nG8zwY8PNQMgHE9aFk.7x./UCpUENO3r5JviNReMpn0Vm7VHWvFRG", 1 },
                    { new Guid("d38cca2f-a50d-435e-8fd8-f071617a2061"), new DateTime(2025, 10, 30, 19, 10, 29, 526, DateTimeKind.Utc).AddTicks(3380), "admin@example.com", "$2a$11$msBrPP6Xgh0OO8n5LsqSfekAZiU/ufQ8KqvB.5ZC4HmnVS4NUIhiG", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Tasks");
        }
    }
}
