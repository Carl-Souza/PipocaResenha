using Microsoft.EntityFrameworkCore;
using PipocaResenha.Models;

namespace PipocaResenha.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<MovieCinema> MovieCinemas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<MovieCinema>().HasKey(mc => new { mc.MovieId, mc.CinemaId });
        }
    }
}