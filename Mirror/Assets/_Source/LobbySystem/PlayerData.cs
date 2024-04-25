using TMPro;
using UnityEngine;
using PlayerSystem;

namespace LobbySystem
{
    public class PlayerData : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;

        public void SetPlayer(PlayerController player)
        {
            _playerName.text = player.GetName();
        }
    }
}