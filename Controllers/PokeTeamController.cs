using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using poketeam_api.DbModels;
using System;
using System.Collections.Generic;
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
        private readonly IHostEnvironment hostEnvironment;

        /// <summary />
        public PokeTeamController(IHttpClientFactory clientFactory, IHostEnvironment hostEnvironment)

        {
            this.hostEnvironment = hostEnvironment;
            if (clientFactory == null)
            {
                throw new ArgumentNullException(nameof(clientFactory));
            }
            _client = clientFactory.CreateClient("pokeapi");
        }

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

            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "PokeDatabase")
                .Options;

            using (var context = new ApiContext(options))
            {
                var pokemonteam = new PokemonTeam
                {
                    name = userName,
                    birthDay = userBirthDay,
                    birthMonth = userBirthMonth,
                    birthYear = userBirthYear,
                };
                context.PokemonTeam.Add(pokemonteam);
                await GetPokemon(pokemonteam);
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
                .UseInMemoryDatabase(databaseName: "PokeDatabase")
                .Options;

            using (var context = new ApiContext(options))
            {
                var query = context.PokemonTeam
                                    .Where(p => p.name == userName)
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
        public async Task<IActionResult> EditPokemonTeam(int userId, string newName, int newDay, int newMonth, int newYear)
        {
            if (!hostEnvironment.IsDevelopment())
                return NotFound("You do not have permission to execute this command!"
);

            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "PokeDatabase")
                .Options;

            using (var context = new ApiContext(options))
            {
                try
                {
                    var entity = context.PokemonTeam
                                        .Where(p => p.id == userId)
                                        .First();

                    if (newName != null)
                    {
                        if (newName.Length <= 20)
                        {
                            entity.name = newName;
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
                            entity.birthDay = newDay;
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
                            entity.birthMonth = newMonth;
                        }
                        else
                        {
                            return BadRequest("The birth month is invalid (out of range)");
                        }
                    }
                    if (newYear != 0)
                    {
                        if (newYear > 0)
                            entity.birthYear = newYear;
                        else
                        {
                            return BadRequest("The birth year is invalid (out of range)");
                        }
                    }


                    await GetPokemon(entity);
                    context.SaveChanges();
                    return Created(new Uri("https://localhost:44364/PokeTeam/read?userName=" + entity.name), entity);
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
            if (!hostEnvironment.IsDevelopment())
                return NotFound("You do not have permission to execute this command!");

            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "PokeDatabase")
                .Options;

            using (var context = new ApiContext(options))
            {
                try
                {
                    var entity = context.PokemonTeam
                                            .Where(p => p.id == userId)
                                            .First();
                    context.Remove(entity);
                    context.SaveChanges();
                }
                catch (InvalidOperationException)
                {
                    return BadRequest("Invalid ID");
                }
            }
            return NoContent();
        }

        [HttpGet]
        [Route("getpokemon")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetPokemon(PokemonTeam poketeam)
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "PokeDatabase")
                .Options;
            using (var context = new ApiContext(options))
            {
                int seed = poketeam.birthDay + poketeam.birthMonth + poketeam.birthYear;
                Random rand = new Random(seed);
                Pokemon pokemon;
                var res = new HttpResponseMessage();
                List<Pokemon> pokeList = new List<Pokemon>();

                for (int i = 0; i < 6; i++)
                {
                    res = await _client.GetAsync("pokemon/" + rand.Next(0, 905));
                    pokemon = await res.Content.ReadAsAsync<Pokemon>();
                    pokeList.Add(pokemon);
                }
                poketeam.pokemonOne = pokeList[0].name;
                poketeam.pokemonTwo = pokeList[1].name;
                poketeam.pokemonThree = pokeList[2].name;
                poketeam.pokemonFour = pokeList[3].name;
                poketeam.pokemonFive = pokeList[4].name;
                poketeam.pokemonSix = pokeList[5].name;

                context.SaveChanges();
                return Ok();
            }
        }
    }
}
