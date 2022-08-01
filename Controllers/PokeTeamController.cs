using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace poketeam_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokeTeamController : ControllerBase
    {
        private readonly HttpClient _client;
        /// <summary />
        public PokeTeamController(IHttpClientFactory clientFactory)
        {
            if (clientFactory == null)
            {
                throw new ArgumentNullException(nameof(clientFactory));
            }
            _client = clientFactory.CreateClient("pokeapi");
        }
        [HttpGet]
        [Route("raw")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetPokemonCharacters()
        {
            var res = await _client.GetAsync("pokemon?limit=1154");
            var content = await res.Content.ReadAsStringAsync();
            return Ok(content);
        }
    }
}
