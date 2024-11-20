using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTHub.Portal.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RoleBasedAccessControlScopeGesture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Scope",
                table: "AccessControls",
                newName: "ScopeId");

            migrationBuilder.CreateTable(
                name: "Scopes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ParentId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scopes_Scopes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Scopes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessControls_ScopeId",
                table: "AccessControls",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_ParentId",
                table: "Scopes",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessControls_Scopes_ScopeId",
                table: "AccessControls",
                column: "ScopeId",
                principalTable: "Scopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessControls_Scopes_ScopeId",
                table: "AccessControls");

            migrationBuilder.DropTable(
                name: "Scopes");

            migrationBuilder.DropIndex(
                name: "IX_AccessControls_ScopeId",
                table: "AccessControls");

            migrationBuilder.RenameColumn(
                name: "ScopeId",
                table: "AccessControls",
                newName: "Scope");
        }
    }
}
