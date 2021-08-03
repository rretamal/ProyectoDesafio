using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoDesafio.Models
{
    public class Recommendation
    {
        [JsonProperty("mejores")]
        public int[] Suggestion { get; set; }
    }

    public class RecommendationRelated
    {
        [JsonProperty("recomendados")]
        public string[] Suggestion { get; set; }
    }
}
