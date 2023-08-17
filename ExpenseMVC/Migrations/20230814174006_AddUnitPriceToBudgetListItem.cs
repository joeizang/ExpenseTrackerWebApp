﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitPriceToBudgetListItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "BudgetListItem",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "BudgetListItem");
        }
    }
}
