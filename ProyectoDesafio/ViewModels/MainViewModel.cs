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
        public Anime[] SurveyAnimes { get; set; }
        public Anime[] SearchResults { get; set; }
        public bool MustFillSurvey { get; set; }
        public int CurrentUserId { get; set; }
    }
}
