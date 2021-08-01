using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDesafio.Models
{
    [Table("users")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("user_ip")]
        public string UserIp { get; set; }
    }
}
