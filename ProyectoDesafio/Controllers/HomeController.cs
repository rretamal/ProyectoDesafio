using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        private readonly IActionContextAccessor _accessor;

        public HomeController(AnimeContext context, ILogger<HomeController> logger, IActionContextAccessor accessor)
        {
            _context = context;
            _logger = logger;
            _accessor = accessor;
        }

        public IActionResult Index()
        {
            var viewModel = new MainViewModel();

            viewModel = SetMoviesData(viewModel);

            viewModel = GetUserInformation(viewModel);

            return View(viewModel);
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



        private MainViewModel GetUserInformation(MainViewModel viewModel)
        {
            int userId = 0;

            var remoteIpAddress = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();

            if (remoteIpAddress == "::1")
                remoteIpAddress = "127.0.0.1";

            if (HttpContext.Request.Cookies.ContainsKey("user_info"))
            {
                string savedUserId = HttpContext.Request.Cookies["user_info"];

                int.TryParse(savedUserId, out userId);
            }
            else
            {
                int lastId = _context.Ratings.Max(c => c.UserId);
                userId = lastId + 1;

                var newUser = new User()
                {
                    UserId = userId,
                    UserIp = remoteIpAddress
                };

                _context.Add(newUser);
                _context.SaveChanges();

                HttpContext.Response.Cookies.Append("user_info", userId.ToString());
                HttpContext.Response.Cookies.Append("user_ip", remoteIpAddress);
                viewModel.MustFillSurvey = true;
            }

            viewModel.CurrentUserId = userId;

            return viewModel;
        }

        private MainViewModel SetMoviesData(MainViewModel viewModel) {
            var top6Animes = _context.Animes.Where(c => c.ImgUrl != null).OrderByDescending(c => c.Rating).Take(3).ToList();

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
                var topMovies = _context.Animes.Where(c => c.ImgUrl != null && c.TypeId == movieType.TypeId).OrderByDescending(c => c.Rating).Take(3).ToList();

                viewModel.TopMovies = topMovies.ToArray();
            }

            // Anime del día

            // Seleccion aleatoria para encuesta
            var topTotalList = _context.Animes.Where(c => c.ImgUrl != null).OrderByDescending(c => c.Rating).Take(200).ToList();


            int anime1Index = GetRandomValue(topTotalList, new int[] { });
            int anime2Index = GetRandomValue(topTotalList, new int[] { anime1Index });
            int anime3Index = GetRandomValue(topTotalList, new int[] { anime1Index, anime2Index });
            int anime4Index = GetRandomValue(topTotalList, new int[] { anime1Index, anime2Index, anime3Index });

            var indexes = new Anime[] { topTotalList[anime1Index], topTotalList[anime2Index], topTotalList[anime3Index], topTotalList[anime4Index] };

            viewModel.SurveyAnimes = indexes;

            return viewModel;
        }

        private int GetRandomValue(List<Anime> list, int[] excludedIndexes)
        {
            var animeRandom = new Random();
            
            bool animeExit = false;
            int animeIndex = 0;

            while (!animeExit) {
                animeIndex = animeRandom.Next(list.Count);

                if (!excludedIndexes.Contains(animeIndex))
                {
                    animeExit = true;
                }
            }

            return animeIndex;
        }
    }
}
