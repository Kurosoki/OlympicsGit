using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Olympics.Metier.Business;

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
        public DbSet<cTicket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<cUtilisateurBase>()
                .HasKey(u => u.IDClient); // Configuration explicite de la clé primaire

            modelBuilder.Entity<cPanierBase>()
                .HasKey(p => p.IDPanier);

            modelBuilder.Entity<cPanierBase>()
                .HasMany(p => p.Tickets)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade); 
            // La suppression en cascade est configurée pour que les tickets associés
            // à un panier soient également supprimés lorsque le panier est supprimé.

            modelBuilder.Entity<cTicket>()
                .HasKey(t => t.IDTicket);
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
