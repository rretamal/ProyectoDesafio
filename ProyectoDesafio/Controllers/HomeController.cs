using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProyectoDesafio.Data;
using ProyectoDesafio.Models;
using ProyectoDesafio.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoDesafio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AnimeContext _context;

        public HomeController(AnimeContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var viewModel = new MainViewModel();

            var top6Animes = _context.Animes.Where(c => c.ImgUrl != null).OrderByDescending(c => c.Rating).Take(6).ToList();

            viewModel.Top3 = top6Animes.ToArray();

            // Populares
            var top10Animes = _context.Animes.Where(c => c.ImgUrl != null).OrderByDescending(c => c.Rating).Take(10).ToList();
            viewModel.TopAnimes = top10Animes.ToArray();

            // Mangas Poulares
            var tvType = _context.AnimesTypes.Where(c => c.TypeName.ToLower() == "tv").FirstOrDefault();

            if (tvType != null)
            {
                var topMangas = _context.Animes.Where(c => c.ImgUrl != null && c.TypeId == tvType.TypeId).OrderByDescending(c => c.Rating).Take(3).ToList();
                viewModel.TopMangas = topMangas.ToArray();

                var random = new Random();

                var topListAnimes = _context.Animes.Where(c => c.ImgUrl != null && c.TypeId == tvType.TypeId).OrderByDescending(c => c.Rating).Take(200).ToList();
                int index = random.Next(topListAnimes.Count);
                var dayAnime = topListAnimes[index];

                viewModel.DayAnime = dayAnime;
            }

            // Peliculas populares
            var movieType = _context.AnimesTypes.Where(c => c.TypeName.ToLower() == "movie").FirstOrDefault();

            if (movieType != null)
            {
                var topMovies = _context.Animes.Where(c => c.ImgUrl != null && c.TypeId == tvType.TypeId).OrderByDescending(c => c.Rating).Take(3).ToList();

                viewModel.TopMovies = topMovies.ToArray();
            }

            // Anime del día

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
