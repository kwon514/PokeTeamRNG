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
        /// <param name="userName">The user's name</param>
        /// <param name="userBirthDay">The user's day of birth, which must be a positive integer between 1 to 31 inclusive</param>
        /// <param name="userBirthMonth">The user's month of birth, which must be an integer between 1 to 12 inclusive</param>
        /// <param name="userBirthYear">The user's year of birth, which must be a positive integer</param>
        /// <returns>The user's details and generated pokemon team ids</returns>

        [HttpGet]
        [Route("create")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> CreatePokemonTeam(String userName, int userBirthDay, int userBirthMonth, int userBirthYear)
        {
            if (userBirthYear <= 0 || userBirthMonth < 1 || userBirthMonth > 12 || userBirthDay < 1 || userBirthDay > 31) return BadRequest("The birthdate is invalid (out of range)");
            if (userName.Length > 20) return BadRequest("The name is too long (exceeds 20 characters)");

            int seed = userBirthDay + userBirthMonth + userBirthYear;
            var rand = new Random(seed);

            int pokemonOne = rand.Next(0, 905);
            int pokemonTwo = rand.Next(0, 905);
            int pokemonThree = rand.Next(0, 905);
            int pokemonFour = rand.Next(0, 905);
            int pokemonFive = rand.Next(0, 905);
            int pokemonSix = rand.Next(0, 905);

            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            using (var context = new ApiContext(options))
            {
                var pokemonteam = new PokemonTeam
                {
                    Name = userName,
                    BirthDay = userBirthDay,
                    BirthMonth = userBirthMonth,
                    BirthYear = userBirthYear,
                    pokemonOne = pokemonOne,
                    pokemonTwo = pokemonTwo,
                    pokemonThree = pokemonThree,
                    pokemonFour = pokemonFour,
                    pokemonFive = pokemonFive,
                    pokemonSix = pokemonSix,
                };
                context.PokemonTeam.Add(pokemonteam);
                context.SaveChanges();
                return Created(new Uri("https://localhost:44364/PokeTeam/read?userName=" + userName), pokemonteam);
            };
        }

        /// <summary>
        /// Fetches data for specified user's name
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns>The fetched data for a specified user</returns>
        [HttpGet]
        [Route("read")]
        [ProducesResponseType(200)]
        public IActionResult GetPokemonTeam(String userName)
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            using (var context = new ApiContext(options))
            {
                var query = context.PokemonTeam
                                    .Where(p => p.Name == userName)
                                    .ToList();
                return Ok(query);
            }
        }

        /// <summary>
        /// Updates user information
        /// </summary>
        /// <param name="userId">Enter the ID of the user you would like to update</param>
        /// <param name="newName">Enter a new name or leave blank if you do not want to change</param> 
        /// <param name="newDay">Enter a new birth day or leave blank if you do not want to change</param>
        /// <param name="newMonth">Enter a new birth month or leave blank if you do not want to change</param>
        /// <param name="newYear">Enter a new birth year or leave blank if  you do not want to change</param>
        /// <returns>The updated data for the specified user</returns>
        [HttpPut]
        [Route("update")]
        [ProducesResponseType(201)]
        public IActionResult EditPokemonTeam(int userId, string newName, int newDay, int newMonth, int newYear)
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            using (var context = new ApiContext(options))
            {
                try
                {
                    var entity = context.PokemonTeam
                                        .Where(p => p.Id == userId)
                                        .First();

                    if (newName != null)
                    {
                        if (newName.Length <= 20)
                        {
                            entity.Name = newName;
                        }
                        else
                        {
                            return BadRequest("The name is too long (exceeds 20 characters)");
                        }
                    }
                    if (newDay != 0)
                    {
                        if (newDay > 0 && newDay <= 31)
                        {
                            entity.BirthDay = newDay;
                        }
                        else
                        {
                            return BadRequest("The birth day is invalid (out of range)");
                        }
                    }
                    if (newMonth != 0)
                    {
                        if (newMonth > 0 && newMonth <= 12)
                        {
                            entity.BirthMonth = newMonth;
                        }
                        else
                        {
                            return BadRequest("The birth month is invalid (out of range)");
                        }
                    }
                    if (newYear != 0)
                    {
                        if (newYear > 0)
                            entity.BirthYear = newYear;
                        else
                        {
                            return BadRequest("The birth year is invalid (out of range)");
                        }
                    }

                    int seed = entity.BirthDay + entity.BirthMonth + entity.BirthYear;
                    var rand = new Random(seed);

                    entity.pokemonOne = rand.Next(0, 905);
                    entity.pokemonTwo = rand.Next(0, 905);
                    entity.pokemonThree = rand.Next(0, 905);
                    entity.pokemonFour = rand.Next(0, 905);
                    entity.pokemonFive = rand.Next(0, 905);
                    entity.pokemonSix = rand.Next(0, 905);

                    context.SaveChanges();
                    return Created(new Uri("https://localhost:44364/PokeTeam/read?userName=" + entity.Name), entity);
                }   
                catch (InvalidOperationException)
                {
                    return BadRequest("Invalid ID");
                }
            }

        }
        /// <summary>
        /// Deletes a specified user
        /// </summary>
        /// <param name="userId">The ID of the user you would like to delete</param>
        /// <returns>A 204 No Content Response</returns>
        [HttpDelete]
        [Route("delete")]
        [ProducesResponseType(204)]
        public IActionResult DeletePokemonTeam(int userId)
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            using (var context = new ApiContext(options))
            {
                var entity = context.PokemonTeam
                                        .Where(p => p.Id == userId)
                                        .First();
                context.Remove(entity);
                context.SaveChanges();
            }
            return NoContent();
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
