using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Database.Model
{
    public partial class ThixioContext : DbContext
    {
        public ThixioContext()
        {
        }

        public ThixioContext(DbContextOptions<ThixioContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Amigo> Amigo { get; set; }
        public virtual DbSet<Comentario> Comentario { get; set; }
        public virtual DbSet<Publicacion> Publicacion { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=LAPTOP-N9TB03CS;Database=Thixio;persist security info=True;Integrated Security=SSPI");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Amigo>(entity =>
            {
                entity.HasKey(e => new { e.IdUsuario, e.IdAmigo })
                    .HasName("PK_Amigo");

                entity.ToTable("amigo");

                entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

                entity.Property(e => e.IdAmigo).HasColumnName("idAmigo");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Amigo)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_Amigo");
            });

            modelBuilder.Entity<Comentario>(entity =>
            {
                entity.HasKey(e => e.IdComentario)
                    .HasName("PK__Comentar__C74515DAC0C275DC");

                entity.Property(e => e.IdComentario).HasColumnName("idComentario");

                entity.Property(e => e.Contenido)
                    .HasColumnName("contenido")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.FechaHora)
                    .HasColumnName("fechaHora")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdPublicacion).HasColumnName("idPublicacion");

                entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

                entity.HasOne(d => d.IdPublicacionNavigation)
                    .WithMany(p => p.Comentario)
                    .HasForeignKey(d => d.IdPublicacion)
                    .HasConstraintName("FK_Publicacion");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Comentario)
                    .HasForeignKey(d => d.IdUsuario)
                    .HasConstraintName("FK_Usuario");
            });

            modelBuilder.Entity<Publicacion>(entity =>
            {
                entity.HasKey(e => e.IdPublicacion)
                    .HasName("PK__Publicac__BF9D98902164C5F1");

                entity.Property(e => e.IdPublicacion).HasColumnName("idPublicacion");

                entity.Property(e => e.Contenido)
                    .HasColumnName("contenido")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.FechaHora)
                    .HasColumnName("fechaHora")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario)
                    .HasName("PK__Usuario__645723A6C5917A28");

                entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

                entity.Property(e => e.Apellido)
                    .HasColumnName("apellido")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Contraseña)
                    .HasColumnName("contraseña")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Correo)
                    .HasColumnName("correo")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.NombreUsuario)
                    .HasColumnName("nombreUsuario")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Statuss)
                    .HasColumnName("statuss")
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.Telefono)
                    .HasColumnName("telefono")
                    .HasMaxLength(12)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
