using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMeteoriteYearValueType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
         @"ALTER TABLE ""Meteorite"" 
        ALTER COLUMN ""Year"" TYPE integer
        USING EXTRACT(YEAR FROM ""Year"")::integer;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @"ALTER TABLE ""Meteorite"" 
            ALTER COLUMN ""Year"" TYPE timestamp with time zone
            USING MAKE_TIMESTAMP(""Year""::integer, 1, 1, 0, 0, 0) AT TIME ZONE 'UTC';");
        }
    }
}
