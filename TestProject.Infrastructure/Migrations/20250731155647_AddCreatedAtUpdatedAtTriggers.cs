using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtUpdatedAtTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Meteorite",
                nullable: false,
                defaultValueSql: "NOW() AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Meteorite",
                nullable: false,
                defaultValueSql: "NOW() AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTime));

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION update_updated_at_column()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW.""UpdatedAt"" = NOW() AT TIME ZONE 'UTC';
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER trigger_update_updated_at
                BEFORE UPDATE ON ""Meteorite""
                FOR EACH ROW
                EXECUTE FUNCTION update_updated_at_column();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trigger_update_updated_at ON ""Meteorite"";");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS update_updated_at_column();");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Meteorite",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: null);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Meteorite",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: null);
        }
    }
}