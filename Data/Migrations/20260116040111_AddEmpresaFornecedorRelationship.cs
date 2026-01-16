using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpresaFornecedorRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CnpjOrCpf",
                table: "Fornecedores",
                newName: "Cnpj");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Fornecedores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Fornecedores",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rg",
                table: "Fornecedores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Fornecedores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmpresaFornecedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FornecedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaFornecedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresaFornecedores_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpresaFornecedores_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaFornecedores_EmpresaId_FornecedorId",
                table: "EmpresaFornecedores",
                columns: new[] { "EmpresaId", "FornecedorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaFornecedores_FornecedorId",
                table: "EmpresaFornecedores",
                column: "FornecedorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpresaFornecedores");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "Rg",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Fornecedores");

            migrationBuilder.RenameColumn(
                name: "Cnpj",
                table: "Fornecedores",
                newName: "CnpjOrCpf");
        }
    }
}
