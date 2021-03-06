using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProyectoDesafio.Data;
using ProyectoDesafio.Models;
using ProyectoDesafio.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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

        public IActionResult Search(string qr)
        {
            var viewModel = new MainViewModel();

            viewModel = SetMoviesData(viewModel);

            viewModel = GetUserInformation(viewModel);

            viewModel.SearchResults = _context.Animes.Where(c => c.AnimeName.Contains(qr)).ToArray();

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

        Anime[] previousRes = null;

        [HttpPost]
        public async Task<OkObjectResult> SaveResults(int[] movie1, int[] movie2, int[] movie3, int[] movie4)
        {
            Anime[] respuesta = { };

            try
            {
                if(previousRes != null)
                    return Ok(previousRes);

                var cookieUser = HttpContext.Request.Cookies["user_info"];
                int userId = 0;

                int.TryParse(cookieUser, out userId);

                if (userId > 0)
                {
                    _context.Ratings.Add(new Rating()
                    {
                        RatingVal = movie1[0],
                        AnimeId = movie1[1],
                        UserId = userId
                    });

                    _context.Ratings.Add(new Rating()
                    {
                        RatingVal = movie2[0],
                        AnimeId = movie2[1],
                        UserId = userId
                    });

                    _context.Ratings.Add(new Rating()
                    {
                        RatingVal = movie3[0],
                        AnimeId = movie3[1],
                        UserId = userId
                    });

                    _context.Ratings.Add(new Rating()
                    {
                        RatingVal = movie4[0],
                        AnimeId = movie4[1],
                        UserId = userId
                    });

                    _context.SaveChanges();
                }

                using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromMinutes(10)})
                {
                    string json = "{ " +
                                        "\"userId\": \"" + userId.ToString() + "\", " +
                                          "\"animes\": [" + movie1[1].ToString() + "," + movie2[1].ToString() + "," + movie3[1].ToString() + "," + movie4[1].ToString() + "], " +
                                         "\"ratings\": [" + movie1[0].ToString() + "," + movie2[0].ToString() + "," + movie3[0].ToString() + "," + movie4[0].ToString() + "]" +
                                         "}";

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    //var result = await httpClient.PostAsync("http://34.121.140.207:8080/movie", content);
                    var result = await httpClient.PostAsync("http://localhost:8090/movie", content);
                    string resp = await result.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<Recommendation>(resp);
                    respuesta = _context.Animes.Where(c => data.Suggestion.Contains(c.AnimeId)).ToArray();
                    previousRes = respuesta;
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            { 
            }
            return Ok(null);
        }

        [HttpGet]
        public async Task<OkObjectResult> GetRelatedSearch(string query)
        {
            Anime[] respuesta = { };

            try
            {

                using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromMinutes(10) })
                {
                    string json = "{ " +
                                        "\"movieName\": \"" + query + "\"" +
                                         "}";

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    //var result = await httpClient.PostAsync("http://34.121.140.207:8080/moviesItems", content);
                    var result = await httpClient.PostAsync("http://localhost:8090/moviesItems", content);
                    string resp = await result.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<RecommendationRelated>(resp);
                    respuesta = _context.Animes.Where(c => data.Suggestion.Contains(c.AnimeName)).ToArray();
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
            }
            return Ok(null);
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
                viewModel.MustFillSurvey = true;

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
