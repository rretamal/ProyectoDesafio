using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDesafio.Models
{
    [Table("rating")]
    public class Rating
    {
        [Column("rating_id")]
        public int RatingId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("anime_id")]
        public int AnimeId { get; set; }

        [Column("rating")]
        public int RatingVal { get; set; }
    }
}
