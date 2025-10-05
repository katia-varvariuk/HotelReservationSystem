using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelCatalog.Dal.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "discountcategories",
                columns: table => new
                {
                    discountid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    discountpercent = table.Column<decimal>(type: "numeric(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discountcategories", x => x.discountid);
                });

            migrationBuilder.CreateTable(
                name: "roomcategories",
                columns: table => new
                {
                    categoryid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roomcategories", x => x.categoryid);
                });

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    serviceid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.serviceid);
                });

            migrationBuilder.CreateTable(
                name: "clientdiscounts",
                columns: table => new
                {
                    clientid = table.Column<int>(type: "integer", nullable: false),
                    discountid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientdiscounts", x => new { x.clientid, x.discountid });
                    table.ForeignKey(
                        name: "FK_clientdiscounts_discountcategories_discountid",
                        column: x => x.discountid,
                        principalTable: "discountcategories",
                        principalColumn: "discountid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roomservices",
                columns: table => new
                {
                    categoryid = table.Column<int>(type: "integer", nullable: false),
                    serviceid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roomservices", x => new { x.categoryid, x.serviceid });
                    table.ForeignKey(
                        name: "FK_roomservices_roomcategories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "roomcategories",
                        principalColumn: "categoryid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_roomservices_services_serviceid",
                        column: x => x.serviceid,
                        principalTable: "services",
                        principalColumn: "serviceid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "discountcategories",
                columns: new[] { "discountid", "discountpercent", "name" },
                values: new object[,]
                {
                    { 1, 10.00m, "Student" },
                    { 2, 15.00m, "Senior" },
                    { 3, 20.00m, "VIP" }
                });

            migrationBuilder.InsertData(
                table: "roomcategories",
                columns: new[] { "categoryid", "description", "name" },
                values: new object[,]
                {
                    { 1, "Standard room with basic amenities", "Standard" },
                    { 2, "Deluxe room with premium amenities", "Deluxe" },
                    { 3, "Luxury suite with extra space", "Suite" }
                });

            migrationBuilder.InsertData(
                table: "services",
                columns: new[] { "serviceid", "description", "name", "price" },
                values: new object[,]
                {
                    { 1, "High-speed internet", "WiFi", 0.00m },
                    { 2, "Continental breakfast", "Breakfast", 150.00m },
                    { 3, "Underground parking", "Parking", 100.00m },
                    { 4, "Spa and wellness center", "Spa", 500.00m },
                    { 5, "Swimming pool access", "Pool", 200.00m }
                });

            migrationBuilder.InsertData(
                table: "roomservices",
                columns: new[] { "categoryid", "serviceid" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 3 },
                    { 2, 1 },
                    { 2, 2 },
                    { 2, 3 },
                    { 2, 5 },
                    { 3, 1 },
                    { 3, 2 },
                    { 3, 3 },
                    { 3, 4 },
                    { 3, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_clientdiscounts_discountid",
                table: "clientdiscounts",
                column: "discountid");

            migrationBuilder.CreateIndex(
                name: "IX_discountcategories_name",
                table: "discountcategories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roomcategories_name",
                table: "roomcategories",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_roomservices_serviceid",
                table: "roomservices",
                column: "serviceid");

            migrationBuilder.CreateIndex(
                name: "IX_services_name",
                table: "services",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clientdiscounts");

            migrationBuilder.DropTable(
                name: "roomservices");

            migrationBuilder.DropTable(
                name: "discountcategories");

            migrationBuilder.DropTable(
                name: "roomcategories");

            migrationBuilder.DropTable(
                name: "services");
        }
    }
}
