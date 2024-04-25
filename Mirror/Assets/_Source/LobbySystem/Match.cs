using System.Collections.Generic;
using PlayerSystem;

namespace LobbySystem
{
    [System.Serializable]
    public class Match
    {
        public string ID { get; private set; }
        public List<PlayerController> Players { get; private set; }
        public bool PublicMatch { get; set; }
        public bool InMatch { get; set; }
        public bool MatchFull { get; set; }

        public Match() { }

        public Match(string id, PlayerController player, bool publicMatch)
        {
            Players = new List<PlayerController>();
            ID = id;
            PublicMatch = publicMatch;
            Players.Add(player);
        }
    }
}