using System.Collections.Generic;

namespace poketeam_api.DbModels
{
    public class PokemonTeam
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }
        public int pokemonOne { get; set; }
        public int pokemonTwo { get; set; }
        public int pokemonThree { get; set; }
        public int pokemonFour { get; set; }
        public int pokemonFive { get; set; }
        public int pokemonSix { get; set; }

    }
}
