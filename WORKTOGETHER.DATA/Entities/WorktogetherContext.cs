using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace WORKTOGETHER.DATA.Entities;

public partial class WorktogetherContext : DbContext
{
    public WorktogetherContext()
    {
    }

    public WorktogetherContext(DbContextOptions<WorktogetherContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Baie> Baies { get; set; }

    public virtual DbSet<Commande> Commandes { get; set; }

    public virtual DbSet<DoctrineMigrationVersion> DoctrineMigrationVersions { get; set; }

    public virtual DbSet<Intervention> Interventions { get; set; }

    public virtual DbSet<MessengerMessage> MessengerMessages { get; set; }

    public virtual DbSet<Offre> Offres { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<TicketSupport> TicketSupports { get; set; }

    public virtual DbSet<Unite> Unites { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=worktogether;uid=root;port=3306", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.3.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Baie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("baie")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CapaciteTotale).HasColumnName("capacite_totale");
            entity.Property(e => e.NumeroBaie)
                .HasMaxLength(20)
                .HasColumnName("numero_baie");
        });

        modelBuilder.Entity<Commande>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("commande")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ClientId, "IDX_6EEAA67D19EB6921");

            entity.HasIndex(e => e.OffreId, "IDX_6EEAA67D4CC8505A");

            entity.HasIndex(e => e.ReservationId, "IDX_6EEAA67DB83297E7");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DateCommande)
                .HasColumnType("datetime")
                .HasColumnName("date_commande");
            entity.Property(e => e.DateDebutService).HasColumnName("date_debut_service");
            entity.Property(e => e.DateFinService).HasColumnName("date_fin_service");
            entity.Property(e => e.MontantTotal)
                .HasPrecision(10, 2)
                .HasColumnName("montant_total");
            entity.Property(e => e.MontantTva)
                .HasPrecision(10, 2)
                .HasColumnName("montant_tva");
            entity.Property(e => e.NumeroCommande)
                .HasMaxLength(100)
                .HasColumnName("numero_commande");
            entity.Property(e => e.OffreId).HasColumnName("offre_id");
            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.StatutPaiement)
                .HasMaxLength(255)
                .HasColumnName("statut_paiement");
            entity.Property(e => e.StripeCardBrand)
                .HasMaxLength(50)
                .HasColumnName("stripe_card_brand");
            entity.Property(e => e.StripeCardLast4)
                .HasMaxLength(4)
                .HasColumnName("stripe_card_last4");
            entity.Property(e => e.StripePaymentId)
                .HasMaxLength(255)
                .HasColumnName("stripe_payment_id");
            entity.Property(e => e.TypePaiement)
                .HasMaxLength(255)
                .HasColumnName("type_paiement");

            entity.HasOne(d => d.Client).WithMany(p => p.Commandes)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_6EEAA67D19EB6921");

            entity.HasOne(d => d.Offre).WithMany(p => p.Commandes)
                .HasForeignKey(d => d.OffreId)
                .HasConstraintName("FK_6EEAA67D4CC8505A");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Commandes)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_6EEAA67DB83297E7");
        });

        modelBuilder.Entity<DoctrineMigrationVersion>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("PRIMARY");

            entity.ToTable("doctrine_migration_versions");

            entity.Property(e => e.Version)
                .HasMaxLength(191)
                .HasColumnName("version");
            entity.Property(e => e.ExecutedAt)
                .HasColumnType("datetime")
                .HasColumnName("executed_at");
            entity.Property(e => e.ExecutionTime).HasColumnName("execution_time");
        });

        modelBuilder.Entity<Intervention>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("intervention")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.UniteId, "IDX_D11814ABEC4A74AB");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateDebut)
                .HasColumnType("datetime")
                .HasColumnName("date_debut");
            entity.Property(e => e.DateFin)
                .HasColumnType("datetime")
                .HasColumnName("date_fin");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Statut)
                .HasMaxLength(255)
                .HasColumnName("statut");
            entity.Property(e => e.Titre)
                .HasMaxLength(255)
                .HasColumnName("titre");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UniteId).HasColumnName("unite_id");

            entity.HasOne(d => d.Unite).WithMany(p => p.Interventions)
                .HasForeignKey(d => d.UniteId)
                .HasConstraintName("FK_D11814ABEC4A74AB");
        });

        modelBuilder.Entity<MessengerMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("messenger_messages")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => new { e.QueueName, e.AvailableAt, e.DeliveredAt, e.Id }, "IDX_75EA56E0FB7336F0E3BD61CE16BA31DBBF396750");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvailableAt)
                .HasColumnType("datetime")
                .HasColumnName("available_at");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveredAt)
                .HasColumnType("datetime")
                .HasColumnName("delivered_at");
            entity.Property(e => e.Headers).HasColumnName("headers");
            entity.Property(e => e.QueueName)
                .HasMaxLength(190)
                .HasColumnName("queue_name");
        });

        modelBuilder.Entity<Offre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("offre")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.NomOffre)
                .HasMaxLength(255)
                .HasColumnName("nom_offre");
            entity.Property(e => e.NombreUnites).HasColumnName("nombre_unites");
            entity.Property(e => e.PrixAnnuelle)
                .HasPrecision(10, 2)
                .HasColumnName("prix_annuelle");
            entity.Property(e => e.PrixMensuel)
                .HasPrecision(10, 2)
                .HasColumnName("prix_mensuel");
            entity.Property(e => e.ReductionAnnuelle).HasColumnName("reduction_annuelle");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("reservation")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ClientId, "IDX_42C8495519EB6921");

            entity.HasIndex(e => e.OffreId, "IDX_42C849554CC8505A");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DateDebut).HasColumnName("date_debut");
            entity.Property(e => e.DateFin).HasColumnName("date_fin");
            entity.Property(e => e.OffreId).HasColumnName("offre_id");
            entity.Property(e => e.PrixTotal).HasColumnName("prix_total");

            entity.HasOne(d => d.Client).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_42C8495519EB6921");

            entity.HasOne(d => d.Offre).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.OffreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_42C849554CC8505A");
        });

        modelBuilder.Entity<TicketSupport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("ticket_support")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ClientId, "IDX_8CC8B2F719EB6921");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("date_creation");
            entity.Property(e => e.DateFermeture)
                .HasColumnType("datetime")
                .HasColumnName("date_fermeture");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.NumeroTicket)
                .HasMaxLength(255)
                .HasColumnName("numero_ticket");
            entity.Property(e => e.Priorite).HasColumnName("priorite");
            entity.Property(e => e.Sujet)
                .HasMaxLength(255)
                .HasColumnName("sujet");

            entity.HasOne(d => d.Client).WithMany(p => p.TicketSupports)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_8CC8B2F719EB6921");
        });

        modelBuilder.Entity<Unite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("unite")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.BaieId, "IDX_1D64C11843375062");

            entity.HasIndex(e => e.ReservationId, "IDX_1D64C118B83297E7");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BaieId).HasColumnName("baie_id");
            entity.Property(e => e.Etat)
                .HasMaxLength(255)
                .HasColumnName("etat");
            entity.Property(e => e.NomUnite)
                .HasMaxLength(255)
                .HasColumnName("nom_unite");
            entity.Property(e => e.NumeroUnite)
                .HasMaxLength(255)
                .HasColumnName("numero_unite");
            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.Statut)
                .HasMaxLength(255)
                .HasColumnName("statut");

            entity.HasOne(d => d.Baie).WithMany(p => p.Unites)
                .HasForeignKey(d => d.BaieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_1D64C11843375062");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Unites)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_1D64C118B83297E7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("user")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Email, "UNIQ_IDENTIFIER_EMAIL").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Actif).HasColumnName("actif");
            entity.Property(e => e.Adresse)
                .HasMaxLength(255)
                .HasColumnName("adresse");
            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("date_creation");
            entity.Property(e => e.Email)
                .HasMaxLength(180)
                .HasColumnName("email");
            entity.Property(e => e.IsVerified).HasColumnName("is_verified");
            entity.Property(e => e.Nom)
                .HasMaxLength(200)
                .HasColumnName("nom");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Pays)
                .HasMaxLength(255)
                .HasColumnName("pays");
            entity.Property(e => e.Prenom)
                .HasMaxLength(200)
                .HasColumnName("prenom");
            entity.Property(e => e.Roles)
                .HasColumnType("json")
                .HasColumnName("roles");
            entity.Property(e => e.Societe)
                .HasMaxLength(255)
                .HasColumnName("societe");
            entity.Property(e => e.Telephone)
                .HasMaxLength(30)
                .HasColumnName("telephone");
            entity.Property(e => e.Ville)
                .HasMaxLength(255)
                .HasColumnName("ville");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
