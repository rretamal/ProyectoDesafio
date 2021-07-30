using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDesafio.Models
{
    [Table("anime")]
    public class Anime
    {
        [Column("anime_id")]
        public int AnimeId { get; set; }

        [Column("anime_name")]
        public string AnimeName { get; set; }

        [Column("type_id")]
        public int TypeId { get; set; }

        [Column("episodes")]
        public int Episodes { get; set; }

        [Column("rating")]
        public double Rating { get; set; }

        [Column("members")]
        public int Members { get; set; }

        [Column("img_url")]
        public string ImgUrl { get; set; }

        [ForeignKey("TypeId")]
        public virtual AnimeType AnimeType { get; set; }

        
        public virtual ICollection<AnimeGenre> Genres { get; set; }
    }
}
