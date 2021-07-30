using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDesafio.Models
{
    [Table("anime_genre")]
    public class AnimeGenre
    {
        [Column("anime_genre_id")]
        public int AnimeGenreId { get; set; }

        [Column("anime_id")]
        public int AnimeId { get; set; }

        [Column("genre_id")]
        public int GenreId { get; set; }

        public Genre Genre { get; set; }

        public Anime Anime { get; set; }
    }
}
