using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreApi.Controllers
{
    //https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        IList<Element> _Elements;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _Elements = new List<Element>();
            _Elements.Add(new Element() { Id = 1, Language = "en", Name = "Army" });
            _Elements.Add(new Element() { Id = 1, Language = "fr", Name = "Armée" });
            _Elements.Add(new Element() { Id = 2, Language = "en", Name = "Air" });
            _Elements.Add(new Element() { Id = 2, Language = "fr", Name = "Aviation" });
            _Elements.Add(new Element() { Id = 3, Language = "en", Name = "Sea" });
            _Elements.Add(new Element() { Id = 3, Language = "fr", Name = "Marine" });

     
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("Elements/{language}", Name = "Get Elements")]
        public IEnumerable<Element> GetRanks(string language)
        {
            return _Elements.Where(o => o.Language == language).ToArray();         
        }
    }

    public class Element
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
    }
}
