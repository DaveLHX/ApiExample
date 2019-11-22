using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using System.Security.Principal;

namespace CoreApi.Controllers
{
    //https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api
    //https://jwt.io/
    //http://jwtbuilder.jamiekurtz.com/
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApiTestController : ControllerBase
    {
        IList<Element> _Elements;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ApiTestController> _logger;

        public ApiTestController(ILogger<ApiTestController> logger)
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
        //[AllowAnonymous]
        //[HttpGet]      
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
        [AllowAnonymous]
        [HttpGet("GetAJwtToken/{name}", Name = "Get Token For test")]
        public User GetToken(string name)
        {          
           return  Authenticate(name);
        }


        [AllowAnonymous]
        [HttpGet("Elements/{language}", Name = "Get Elements")]
        public IEnumerable<Element> GetElements(string language)
        {
            return _Elements.Where(o => o.Language == language).ToArray();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Welcome to the api.  Check the /Swagger endpoint");
        }


        [AllowAnonymous]
        [HttpGet("Users", Name = "Get Users")]
        public IEnumerable<User> GetUsers()
        {
            return _users;
        }


        [HttpGet("UsersSecured", Name = "Get Users Secured")]
        public IActionResult GetUsersSecured()
        {
            return Ok(_users);
        }
        [HttpGet("UsersShowClaim", Name = "Get JWT claim")]
        public IActionResult GetClaimInfo()
        {
            var t=new tmp();
            t.Claims = User.Claims.Select((o) =>
            {
               return  new ClaimInfo()
                {
                    Type = o.Type,
                    Value = o.Value,
                    Issuer = o.Issuer
                };
            });
             t.Identity= User.Identity;
           t.IsEvilHacker= User.IsInRole("Evil Hacker");
           t.IsSuperAwesomeAdmin= User.IsInRole("SUPER AWESOME ADMIN");
            return Ok(t);
        }

        public class tmp
        {
            public IEnumerable<ClaimInfo> Claims { get; set; }
            public IIdentity Identity { get; set; }
            public bool IsEvilHacker { get; set; }
            public bool IsSuperAwesomeAdmin { get; set; }
        }
        public class ClaimInfo
        {
            public string Type { get; set; }
            public string Value { get; set; }
            public string Issuer { get; set; }
        }

        private List<User> _users = new List<User>
        {
             new User { Id = 1, FirstName = "Joe", LastName = "Smosh", Username = "JS" },
             new User { Id = 2, FirstName = "Richard", LastName = "Feynman", Username = "WhatdoYouCarePeopleThink" },
             new User { Id = 3, FirstName = "John", LastName = "Wick", Username = "JohnyBoy" },
        };




        private User Authenticate(string name)
        {
            var fakeUser = new User { Id = 99, FirstName = name, LastName = "LastName", Username = $"{name}.LastName"};

            //IdentityModelEventSource.ShowPII = true;          
            var key = Encoding.ASCII.GetBytes("hello this is my scret key I hope you like and enjoy it very much");
           
            var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, fakeUser.Username),                   
                    new Claim(ClaimTypes.GivenName, fakeUser.FirstName),
                    new Claim(ClaimTypes.SerialNumber, "123AWEVE"),
                    new Claim("My custom claim Type", "Can juggle")
                };
            if(name=="Dave")
            {
                claims.Add(new Claim(ClaimTypes.Role, "SUPER AWESOME ADMIN"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "Evil Hacker"));
            }           
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer="My Authentication API",
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            fakeUser.Token = tokenHandler.WriteToken(token);
            return fakeUser;
           
        }
    }

    public class Element
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }       
        public string Token { get; set; }
    }
}
