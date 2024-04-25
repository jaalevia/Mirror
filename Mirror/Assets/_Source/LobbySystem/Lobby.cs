using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;
using Mirror;

namespace LobbySystem
{
    public class Lobby : MonoBehaviour
    {
        [field: SerializeField] public NetworkMatch NetworkMatch { get; private set; }

        private List<PlayerController> _players = new List<PlayerController>();

        public void AddPlayer(PlayerController player)
        {
            _players.Add(player);
        }
    }
}