using UnityEngine;

namespace LobbySystem
{
    public class LobbyStarter : MonoBehaviour
    {
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _spawnPoints;
        [SerializeField] private GameObject _inputListener;
        [SerializeField] private GameObject _gameUi;

        private void Start()
        {
            _mainMenu.SetActive(true);
            _spawnPoints.SetActive(true);
            _inputListener.SetActive(true);
            _gameUi.SetActive(true);

            DontDestroyOnLoad(_mainMenu);
            DontDestroyOnLoad(_spawnPoints);
            DontDestroyOnLoad(_inputListener);
            DontDestroyOnLoad(_gameUi);
        }
    }
}