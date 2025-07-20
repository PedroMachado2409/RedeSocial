using Microsoft.EntityFrameworkCore;
using RedeSocial.Domain.Entities;

namespace RedeSocial.Infraestrutura.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<AmizadePendente> AmizadePendentes { get; set; }
        public DbSet<Amizade> Amizades { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet <Post> Posts { get; set; }
       

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Amizade>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Amizade>()
                .HasOne(a => a.Amigo)
                .WithMany()
                .HasForeignKey(a => a.AmigoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
