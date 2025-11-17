using Microsoft.EntityFrameworkCore;
using PipocaResenha.Models;

namespace PipocaResenha.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuarios> Usuario { get; set; }
        public DbSet<Filmes> Filmes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cinemas> Cinemas { get; set; }
        public DbSet<FilmesCinemas> FilmesCinemas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FilmesCinemas>().HasKey(mc => new { mc.CodigoFilme, mc.CodigoCinema });
        }
    }
}