using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IaEapp.Migrations
{
    /// <inheritdoc />
    public partial class addedincomeintransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Income",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Income",
                table: "Transactions");
        }
    }
}
