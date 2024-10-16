using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Olympics.Metier.Models;

namespace Olympics.Database
{
    //ApplicationDbContext hérite de DbContext, qui est la classe de base pour toutes les interactions avec la base de données.
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<cUtilisateurBase> Utilisateurs { get; set; }
        public DbSet<cPanierBase> Panier { get; set; }
        public DbSet<cOffresBase> Offres { get; set; }
        public DbSet<cTicket> Tickets { get; set; }
        public DbSet<cPayementBase> Payement { get; set; }
        public DbSet<cPanierArchive> PanierArchive { get; set; }
        public DbSet<cPanierArchive> TicketsArchive { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<cUtilisateurBase>()
                .HasKey(u => u.IDClient); // Configuration explicite de la clé primaire

            modelBuilder.Entity<cPanierBase>()
                .HasKey(p => p.IDPanier);

            modelBuilder.Entity<cPanierBase>()
                .HasMany(t => t.Tickets)
                .WithOne(p => p.Panier) // Ajoute la propriété de navigation
                .HasForeignKey(pt => pt.IDPanier) // Configure la clé étrangère
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<cPayementBase>()
                .HasKey(pa => pa.IDPayement); 

            modelBuilder.Entity<cPayementBase>()
                .HasOne(p => p.Panier) // Configure la relation avec cPanierBase
                .WithMany() // Indique que cPanierBase n'a pas besoin d'une collection de cPayementBase
                .HasForeignKey(pt => pt.IDPanier) 
                .OnDelete(DeleteBehavior.Cascade); // Comportement lors de la suppression

            modelBuilder.Entity<cOffresBase>()
                .HasKey(o => o.IDOffre);

            // Configuration pour cPanierArchive
            modelBuilder.Entity<cPanierArchive>()
                .HasKey(pa => pa.IDPanierArchive);

            modelBuilder.Entity<cPanierArchive>()
                .HasMany(pa => pa.TicketsArchive)
                .WithOne(t => t.PanierArchive) // Ajoute la propriété de navigation
                .HasForeignKey(t => t.IDPanierArchive) // Configure la clé étrangère
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration pour cTicketArchive
            modelBuilder.Entity<cTicketArchive>()
                .HasKey(ta => ta.IDTicketArchive);

            modelBuilder.Entity<cTicketArchive>()
                .HasOne(ta => ta.PanierArchive)
                .WithMany(pa => pa.TicketsArchive) // Ajoute la collection de cTicketArchive
                .HasForeignKey(ta => ta.IDPanierArchive)
                .OnDelete(DeleteBehavior.Cascade);


            // Configuration des propriétés DateTime sans conversion
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;

                var dateTimeProperties = clrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

                foreach (var property in dateTimeProperties)
                {
                    modelBuilder.Entity(clrType)
                        .Property(property.Name)
                        .HasColumnType("timestamp without time zone"); // Spécifiez le type ici
                }
            }

            base.OnModelCreating(modelBuilder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=DBOlympics;Username=Kurosoki;Password=Sensei971!");
            }
        }


    }
}
