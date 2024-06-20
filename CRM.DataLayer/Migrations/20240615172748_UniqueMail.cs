using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class UniqueMail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_leads_mail",
                table: "leads",
                column: "mail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_leads_mail",
                table: "leads");
        }
    }
}
