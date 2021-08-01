using System;
using Microsoft.EntityFrameworkCore;
using ProyectoDesafio.Models;

namespace ProyectoDesafio.Data
{
    public class AnimeContext: DbContext
    {
        public AnimeContext(DbContextOptions<AnimeContext> options): base(options)
        {
        }

        public DbSet<Anime> Animes { get; set; }
        public DbSet<AnimeGenre> AnimeGenres { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<AnimeType> AnimesTypes { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Rating> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Anime>().ToTable("anime");
            modelBuilder.Entity<AnimeGenre>().ToTable("anime_genre");
            modelBuilder.Entity<AnimeType>().ToTable("anime_type");
            modelBuilder.Entity<Genre>().ToTable("genre");
            modelBuilder.Entity<Rating>().ToTable("rating");
            modelBuilder.Entity<User>().ToTable("users");
        }
    }
}
