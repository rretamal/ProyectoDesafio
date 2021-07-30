using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDesafio.Models
{
    [Table("genre")]
    public class Genre
    {
        [Column("genre_id")]
        public int GenreId { get; set; }

        [Column("genre_name")]
        public int GenreName { get; set; }
    }
}
