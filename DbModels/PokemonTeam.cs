using System.Collections.Generic;

namespace poketeam_api.DbModels
{
    public class PokemonTeam
    {
        public int id { get; set; }
        public string name { get; set; }
        public int birthDay { get; set; }
        public int birthMonth { get; set; }
        public int birthYear { get; set; }
        public string pokemonOne { get; set; }
        public string pokemonTwo { get; set; }
        public string pokemonThree { get; set; }
        public string pokemonFour { get; set; }
        public string pokemonFive { get; set; }
        public string pokemonSix { get; set; }
    }
}