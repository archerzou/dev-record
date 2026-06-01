using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevRecord.Api.Migrations.Application;

/// <inheritdoc />
public partial class Add_Habit_AutomationSource : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "automation_source",
            schema: "dev_record",
            table: "habits",
            type: "integer",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "automation_source",
            schema: "dev_record",
            table: "habits");
    }
}
