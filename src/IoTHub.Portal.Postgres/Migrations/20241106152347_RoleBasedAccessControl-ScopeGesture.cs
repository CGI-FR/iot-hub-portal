// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#nullable disable

namespace IoTHub.Portal.Postgres.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;
    /// <inheritdoc />
    public partial class RoleBasedAccessControlScopeGesture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.RenameColumn(
                name: "Scope",
                table: "AccessControls",
                newName: "ScopeId");

            _ = migrationBuilder.CreateTable(
                name: "Scopes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Father = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Scopes", x => x.Id);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_AccessControls_ScopeId",
                table: "AccessControls",
                column: "ScopeId");

            _ = migrationBuilder.AddForeignKey(
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
            _ = migrationBuilder.DropForeignKey(
                name: "FK_AccessControls_Scopes_ScopeId",
                table: "AccessControls");

            _ = migrationBuilder.DropTable(
                name: "Scopes");

            _ = migrationBuilder.DropIndex(
                name: "IX_AccessControls_ScopeId",
                table: "AccessControls");

            _ = migrationBuilder.RenameColumn(
                name: "ScopeId",
                table: "AccessControls",
                newName: "Scope");
        }
    }
}
