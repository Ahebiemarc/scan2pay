using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using scan2pay.Models;

namespace scan2pay.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<QrCode> QrCodes { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        // --- Les DbSets pour Article, Commande, Facture ont été supprimés ---

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Important pour Identity

            // --- Configuration des relations et contraintes SIMPLIFIÉES ---

            // ApplicationUser <-> Wallet (One-to-One, Wallet dépendant de ApplicationUser)
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.ApplicationUser)
                .HasForeignKey<Wallet>(w => w.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser (Marchand) <-> QrCode (One-to-One)
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.QrCode)
                .WithOne(q => q.Marchand)
                .HasForeignKey<QrCode>(q => q.MarchandId)
                .OnDelete(DeleteBehavior.Cascade);

            // Wallet <-> Transaction (One-to-Many)
            builder.Entity<Wallet>()
                .HasMany(w => w.Transactions)
                .WithOne(t => t.Wallet)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            // QrCode <-> Transaction (One-to-Many, un QR peut avoir plusieurs transactions)
            builder.Entity<QrCode>()
                .HasMany(q => q.Transactions)
                .WithOne(t => t.QrCode)
                .HasForeignKey(t => t.QrCodeId)
                .OnDelete(DeleteBehavior.SetNull);

            // ApplicationUser <-> PaymentMethod (One-to-Many)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.PaymentMethods)
                .WithOne(pm => pm.ApplicationUser)
                .HasForeignKey(pm => pm.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // PaymentMethod <-> Transaction (One-to-Many pour Source et Destination)
            builder.Entity<PaymentMethod>()
                .HasMany(pm => pm.DepositTransactions)
                .WithOne(t => t.SourcePaymentMethod)
                .HasForeignKey(t => t.SourcePaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PaymentMethod>()
                .HasMany(pm => pm.WithdrawalTransactions)
                .WithOne(t => t.DestinationPaymentMethod)
                .HasForeignKey(t => t.DestinationPaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaction <-> Transaction (Self-referencing for Refunds)
            builder.Entity<Transaction>()
                .HasOne(t => t.RelatedTransaction)
                .WithMany()
                .HasForeignKey(t => t.RelatedTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser (Payer/Payee) <-> Transaction
            builder.Entity<Transaction>()
                .HasOne(t => t.Payer)
                .WithMany(u => u.InitiatedTransactions)
                .HasForeignKey(t => t.PayerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne(t => t.Payee)
                .WithMany(u => u.ReceivedTransactions)
                .HasForeignKey(t => t.PayeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser <-> Notification (One-to-Many)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Notifications)
                .WithOne(n => n.ApplicationUser)
                .HasForeignKey(n => n.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);


            // --- Les relations e-commerce ont été supprimées ---

            // Index pour optimiser les recherches fréquentes
            builder.Entity<Wallet>().HasIndex(w => w.ApplicationUserId).IsUnique();
            builder.Entity<Transaction>().HasIndex(t => t.Timestamp);
            builder.Entity<Transaction>().HasIndex(t => t.ReferenceId);
            builder.Entity<QrCode>().HasIndex(q => q.MarchandId).IsUnique(); // Unique car un marchand a un seul QR
            builder.Entity<QrCode>().HasIndex(q => q.QrCodeData).IsUnique();
            builder.Entity<PaymentMethod>().HasIndex(pm => pm.ApplicationUserId);
            builder.Entity<Notification>().HasIndex(n => n.ApplicationUserId);
            builder.Entity<Notification>().HasIndex(n => n.CreatedAt);


            // Configuration des décimales pour PostgreSQL
            builder.Entity<Wallet>().Property(w => w.Balance).HasPrecision(18, 2);
            builder.Entity<Transaction>().Property(t => t.Amount).HasPrecision(18, 2);
        }
    }
}