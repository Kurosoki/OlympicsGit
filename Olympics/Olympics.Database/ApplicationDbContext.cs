using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Olympics.Metier.Business;

namespace Olympics.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<cUtilisateurBase> Utilisateurs { get; set; }

        // Ajoutez d'autres DbSet pour d'autres entités si nécessaire
    }
}
