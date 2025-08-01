using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMeteoriteRecClassExactMatchIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE INDEX idx_meteorite_recclass_exact ON ""Meteorite"" (""RecClass"")");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP INDEX idx_meteorite_recclass_exact");
        }
    }
}
