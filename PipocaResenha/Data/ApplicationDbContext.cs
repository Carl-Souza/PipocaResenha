using Microsoft.EntityFrameworkCore;
using PipocaResenha.Models;

namespace PipocaResenha.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuarios> Usuario { get; set; }
        public DbSet<Filmes> Filme { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cinemas> Cinemas { get; set; }
        public DbSet<FilmesCinema> FilmeCinema { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FilmesCinema>().HasKey(mc => new { mc.CodigoFilme, mc.CodigoCinema });
        }
    }
}