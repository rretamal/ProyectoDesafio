using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDesafio.Models
{
    [Table("anime_type")]
    public class AnimeType
    {
        [Key]
        [Column("type_id")]
        public int TypeId { get; set; }

        [Column("type_name")]
        public string TypeName { get; set; }
    }
}
