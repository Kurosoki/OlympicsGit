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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<cUtilisateurBase>()
                .HasKey(u => u.IDClient); // Configuration explicite de la clé primaire

            modelBuilder.Entity<cPanierBase>()
                .HasKey(p => p.IDPanier);

            modelBuilder.Entity<cPanierBase>()
                .HasMany(p => p.Tickets)
                .WithOne(t => t.Panier) // Ajoute la propriété de navigation
                .HasForeignKey(t => t.IDPanier) // Configure la clé étrangère
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<cPayementBase>()
                .HasKey(p => p.IDPayement); 

            modelBuilder.Entity<cPayementBase>()
                .HasOne(p => p.Panier) // Configure la relation avec cPanierBase
                .WithMany() // Indique que cPanierBase n'a pas besoin d'une collection de cPayementBase
                .HasForeignKey(p => p.IDPanier) 
                .OnDelete(DeleteBehavior.Cascade); // Comportement lors de la suppression

            modelBuilder.Entity<cOffresBase>()
                .HasKey(o => o.IDOffre);

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
