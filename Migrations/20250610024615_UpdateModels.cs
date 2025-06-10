using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace scan2pay.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QrCodes_AspNetUsers_MarchandId",
                table: "QrCodes");

            migrationBuilder.DropTable(
                name: "CommandeArticles");

            migrationBuilder.DropTable(
                name: "Factures");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Commandes");

            migrationBuilder.DropIndex(
                name: "IX_QrCodes_MarchandId",
                table: "QrCodes");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "QrCodes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "QrCodes");

            migrationBuilder.CreateIndex(
                name: "IX_QrCodes_MarchandId",
                table: "QrCodes",
                column: "MarchandId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QrCodes_AspNetUsers_MarchandId",
                table: "QrCodes",
                column: "MarchandId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QrCodes_AspNetUsers_MarchandId",
                table: "QrCodes");

            migrationBuilder.DropIndex(
                name: "IX_QrCodes_MarchandId",
                table: "QrCodes");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "QrCodes",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "QrCodes",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarchandId = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PrixUnitaire = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_AspNetUsers_MarchandId",
                        column: x => x.MarchandId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commandes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    DateCommande = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FactureId = table.Column<int>(type: "integer", nullable: true),
                    QrCodeUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Statut = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commandes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commandes_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommandeArticles",
                columns: table => new
                {
                    CommandeId = table.Column<int>(type: "integer", nullable: false),
                    ArticleId = table.Column<int>(type: "integer", nullable: false),
                    PrixAuMomentDeCommande = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantite = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandeArticles", x => new { x.CommandeId, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_CommandeArticles_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommandeArticles_Commandes_CommandeId",
                        column: x => x.CommandeId,
                        principalTable: "Commandes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Factures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommandeId = table.Column<int>(type: "integer", nullable: false),
                    DateFacture = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MontantTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NumeroFacture = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PdfUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StatutPaiement = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Factures_Commandes_CommandeId",
                        column: x => x.CommandeId,
                        principalTable: "Commandes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QrCodes_MarchandId",
                table: "QrCodes",
                column: "MarchandId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_MarchandId",
                table: "Articles",
                column: "MarchandId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Nom",
                table: "Articles",
                column: "Nom");

            migrationBuilder.CreateIndex(
                name: "IX_CommandeArticles_ArticleId",
                table: "CommandeArticles",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Commandes_ClientId",
                table: "Commandes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Commandes_DateCommande",
                table: "Commandes",
                column: "DateCommande");

            migrationBuilder.CreateIndex(
                name: "IX_Factures_CommandeId",
                table: "Factures",
                column: "CommandeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Factures_NumeroFacture",
                table: "Factures",
                column: "NumeroFacture",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QrCodes_AspNetUsers_MarchandId",
                table: "QrCodes",
                column: "MarchandId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
