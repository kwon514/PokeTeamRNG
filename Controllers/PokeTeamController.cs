using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using poketeam_api.DbModels;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace poketeam_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokeTeamController : ControllerBase
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Determines pokemon team from user's birthday
        /// </summary>
        /// <param name="name">The user's name</param>
        /// <param name="day">The user's day of birth, which must be a positive integer between 1 to 31 inclusive</param>
        /// <param name="month">The user's month of birth, which must be an integer between 1 to 12 inclusive</param>
        /// <param name="year">The user's year of birth, which must be a positive integer</param>
        /// <returns>The user's details and generated pokemon team ids</returns>

        [HttpGet]
        [Route("create")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> GetPokemonCharacter(String name, int day, int month, int year)
        {
            if (year <= 0 || month < 1 || month > 12 || day < 1 || day > 31) return BadRequest("The birthdate is invalid (out of range)");
            if (name.Length > 20) return BadRequest("The name is too long (exceeds 20 characters)");

            int[] pokemonIDs = new int[6];
            int seed = day + month + year;
            var rand = new Random(seed);
            for (int i = 0; i < 6; i++)
            {
                pokemonIDs[i] = rand.Next(0, 905);
            }

            int pokemonOne = pokemonIDs[0];
            int pokemonTwo = pokemonIDs[1];
            int pokemonThree = pokemonIDs[2];
            int pokemonFour = pokemonIDs[3];
            int pokemonFive = pokemonIDs[4];
            int pokemonSix = pokemonIDs[5];

            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            using (var context = new ApiContext(options))
            {
                var pokemonteam = new PokemonTeam
                {
                    Name = name,
                    Seed = seed,
                    pokemonOne = pokemonIDs[0],
                    pokemonTwo = pokemonIDs[1],
                    pokemonThree = pokemonIDs[2],
                    pokemonFour = pokemonIDs[3],
                    pokemonFive = pokemonIDs[4],
                    pokemonSix = pokemonIDs[5],
                };
                context.PokemonTeam.Add(pokemonteam);
                context.SaveChanges();
                return Created(new Uri("https://google.com"), pokemonteam);
            };
        }

        /// <summary>
        /// Fetches data for specified user's name
        /// </summary>
        /// <param name="name">The user's name</param>
        /// <returns>The fetched data for a specified user</returns>
        [HttpGet]
        [Route("read")]
        [ProducesResponseType(200)]
        public IActionResult GetPokemonTeam(String name)
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            using (var context = new ApiContext(options))
            {
                var query = context.PokemonTeam
                                    .Where(p => p.Name == name)
                                    .ToList();
                return Ok(query);
            }
        }


        /// <summary />
        public PokeTeamController(IHttpClientFactory clientFactory)

        {
            if (clientFactory == null)
            {
                throw new ArgumentNullException(nameof(clientFactory));
            }
            _client = clientFactory.CreateClient("pokeapi");
        }
    }
}
