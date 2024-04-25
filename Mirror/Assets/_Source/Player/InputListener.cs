using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    public class InputListener : NetworkBehaviour
    {
        private List<PlayerController> _players = new List<PlayerController>();

        private void Update()
        {
            foreach(PlayerController player in _players)
            {
                if (player.isOwned)
                {
                    player.Move(Input.GetAxis("Horizontal"),
                        Input.GetAxis("Vertical"));
                }
            }
        }

        public void AddListener(PlayerController player)
        {
            _players.Add(player);
        }

        public void RemoveListener(PlayerController player)
        {
            _players.Remove(player);
        }
    }
}