using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Upstart.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    social_security_number = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    address_line_1 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_line_2 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    state = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    zip_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    annual_income = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    employment_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    credit_score = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "loans",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    loan_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    interest_rate = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    term_months = table.Column<int>(type: "integer", nullable: false),
                    monthly_payment = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    loan_purpose = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    loan_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    application_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    approval_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    disbursement_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    maturity_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    outstanding_balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    total_payments_made = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    next_payment_due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    payment_frequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    late_fees = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    origination_fee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    apr = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    loan_officer_notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_entity_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_loans", x => x.id);
                    table.ForeignKey(
                        name: "fk_loans_users_user_entity_id",
                        column: x => x.user_entity_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_loans_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_loans_user_entity_id",
                table: "loans",
                column: "user_entity_id");

            migrationBuilder.CreateIndex(
                name: "ix_loans_user_id",
                table: "loans",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loans");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
