using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalLearningPlatform.Services.UserService.Migrations
{
    /// <inheritdoc />
    public partial class UserProfile_Column_Overview_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Profile_Overview",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profile_Overview",
                table: "Users");
        }
    }
}
