using ProyectoDesafio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoDesafio.ViewModels
{
    public class MainViewModel
    {
        public Anime[] Top3 { get; set; }
        public Anime[] TopAnimes { get; set; }
        public Anime[] TopMangas { get; set; }
        public Anime[] TopMovies { get; set; }
        public Anime DayAnime { get; set; }
    }
}
