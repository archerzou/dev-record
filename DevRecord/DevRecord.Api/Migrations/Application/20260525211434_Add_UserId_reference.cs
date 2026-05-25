using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevRecord.Api.Migrations.Application;

/// <inheritdoc />
public partial class Add_UserId_reference : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DELETE FROM dev_record.habit_tags;
            DELETE FROM dev_record.habits;
            DELETE FROM dev_record.tags;
            """);

        migrationBuilder.DropIndex(
            name: "ix_tags_name",
            schema: "dev_record",
            table: "tags");

        migrationBuilder.AddColumn<string>(
            name: "user_id",
            schema: "dev_record",
            table: "tags",
            type: "character varying(500)",
            maxLength: 500,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "user_id",
            schema: "dev_record",
            table: "habits",
            type: "character varying(500)",
            maxLength: 500,
            nullable: false,
            defaultValue: "");

#pragma warning disable CA1861
        migrationBuilder.CreateIndex(
            name: "ix_tags_user_id_name",
            schema: "dev_record",
            table: "tags",
            columns: new[] { "user_id", "name" },
            unique: true);
#pragma warning restore CA1861

        migrationBuilder.CreateIndex(
            name: "ix_habits_user_id",
            schema: "dev_record",
            table: "habits",
            column: "user_id");

        migrationBuilder.AddForeignKey(
            name: "fk_habits_users_user_id",
            schema: "dev_record",
            table: "habits",
            column: "user_id",
            principalSchema: "dev_record",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_tags_users_user_id",
            schema: "dev_record",
            table: "tags",
            column: "user_id",
            principalSchema: "dev_record",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_habits_users_user_id",
            schema: "dev_record",
            table: "habits");

        migrationBuilder.DropForeignKey(
            name: "fk_tags_users_user_id",
            schema: "dev_record",
            table: "tags");

        migrationBuilder.DropIndex(
            name: "ix_tags_user_id_name",
            schema: "dev_record",
            table: "tags");

        migrationBuilder.DropIndex(
            name: "ix_habits_user_id",
            schema: "dev_record",
            table: "habits");

        migrationBuilder.DropColumn(
            name: "user_id",
            schema: "dev_record",
            table: "tags");

        migrationBuilder.DropColumn(
            name: "user_id",
            schema: "dev_record",
            table: "habits");

        migrationBuilder.CreateIndex(
            name: "ix_tags_name",
            schema: "dev_record",
            table: "tags",
            column: "name",
            unique: true);
    }
}
