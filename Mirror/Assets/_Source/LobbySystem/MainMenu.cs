using UnityEngine;
using Mirror;
using UnityEngine.UI;
using PlayerSystem;
using TMPro;
using System.Collections;

namespace LobbySystem
{
    public class MainMenu : NetworkBehaviour
    {
        [SerializeField] private int _maxPlayers;

        [SerializeField] private Canvas _lobbyCanvas;
        [SerializeField] private Canvas _searchingCanvas;

        [SerializeField] private Button _searchButton;
        [SerializeField] private Button _publicHostButton;
        [SerializeField] private Button _privateHostButton;
        [SerializeField] private Button _joinButton;

        [SerializeField] private Button _beginGameButton;
        [SerializeField] private Button _exitGameButton;
        [SerializeField] private Button _exitSearchingButton;
        [SerializeField] private Button _sendMessage;

        [SerializeField] private TMP_InputField _joinInput;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_InputField _messageInput;

        [SerializeField] private TextMeshProUGUI _idText;
        [SerializeField] private TextMeshProUGUI _chatHistoryText;
        
        [SerializeField] private PlayerData _playerDataPrefab;
        [SerializeField] private Lobby _lobbyPrefab;

        [SerializeField] private Transform _playerDataHolder;

        private readonly SyncList<Match> _matches = new SyncList<Match>();
        private readonly SyncList<string> _matchIDs = new SyncList<string>();

        private bool _searching;
        private PlayerData _localPlayerData;

        private void Awake()
        {
            Bind();
        }

        private void OnDestroy()
        {
            Expose();
        }

        private void OnLevelWasLoaded(int level)
        {
            gameObject.SetActive(false);
        }

        private void Bind()
        {
            _publicHostButton.onClick.AddListener(() => Host(true));
            _privateHostButton.onClick.AddListener(() => Host(false));
            _joinButton.onClick.AddListener(Join);

            _beginGameButton.onClick.AddListener(StartGame);
            _exitGameButton.onClick.AddListener(DisconnectGame);

            _searchButton.onClick.AddListener(SearchGame);
            _exitSearchingButton.onClick.AddListener(CancelSearchGame);
            _sendMessage.onClick.AddListener(HandleMessage);
        }

        private void Expose()
        {
            _publicHostButton.onClick.RemoveListener(() => Host(true));
            _privateHostButton.onClick.RemoveListener(() => Host(false));
            _joinButton.onClick.RemoveListener(Join);

            _beginGameButton.onClick.RemoveListener(StartGame);
            _exitGameButton.onClick.RemoveListener(DisconnectGame);

            _searchButton.onClick.RemoveListener(SearchGame);
            _exitSearchingButton.onClick.RemoveListener(CancelSearchGame);
        }

        private void ChangeButtonsState(bool isOn)
        {
            _joinInput.interactable = isOn;
            _nameInput.interactable = isOn;

            _searchButton.interactable = isOn;
            _publicHostButton.interactable = isOn;
            _privateHostButton.interactable = isOn;
            _joinButton.interactable = isOn;
        }

        private void Host(bool publicHost)
        {
            ChangeButtonsState(false);
            PlayerController.LocalPlayer.HostGame(_nameInput.text, publicHost);
        }

        private void Join()
        {
            ChangeButtonsState(false);
            PlayerController.LocalPlayer.JoinGame(_joinInput.text.ToUpper(), _nameInput.text);
        }

        private void DisconnectGame()
        {
            if (_localPlayerData != null)
            {
                Destroy(_localPlayerData.gameObject);
                _localPlayerData = null;
            }
            PlayerController.LocalPlayer.DisconnectGame();
            _lobbyCanvas.enabled = false;
            ChangeButtonsState(true);
        }

        private void StartGame()
        {
            PlayerController.LocalPlayer.BeginGame();
        }

        private void SearchGame()
        {
            StartCoroutine(Searching());
        }

        private void CancelSearchGame()
        {
            ChangeButtonsState(true);
            _searching = false;
        }

        private void HandleMessage()
        {
            if(!string.IsNullOrWhiteSpace(_messageInput.text))
            {
                PlayerController.LocalPlayer.CmdHandleMessage(_messageInput.text);
            }
        }

        private IEnumerator Searching()
        {
            ChangeButtonsState(false);
            _searchingCanvas.enabled = true;
            _searching = true;

            float searchInterval = 1;
            float currentTime = 1;

            while (_searching)
            {
                if (currentTime > 0)
                    currentTime -= Time.deltaTime;
                else
                {
                    currentTime = searchInterval;
                    PlayerController.LocalPlayer.SearchGame(_nameInput.text);
                }
                yield return null;
            }
            _searchingCanvas.enabled = false;
        }

        public void SetBeginButtonActive(bool isActive)
        {
            _beginGameButton.interactable = isActive;
        }

        public void CheckSuccess(bool success, string matchID, bool isHost)
        {
            if (success)
            {
                _lobbyCanvas.enabled = true;
                _searchingCanvas.enabled = false;
                _searching = false;
                if (_localPlayerData != null)
                {
                    Destroy(_localPlayerData.gameObject);
                    _localPlayerData = null;
                }
                _localPlayerData = SpawnPlayerPrefab(PlayerController.LocalPlayer);
                _idText.text = matchID;
                _beginGameButton.interactable = isHost;
            }
            else
            {
                ChangeButtonsState(true);
            }
        }

        public bool HostGame(string matchID, PlayerController player, bool publicMatch)
        {
            if (!_matchIDs.Contains(matchID))
            {
                _matchIDs.Add(matchID);
                Match newMatch = new Match(matchID, player, publicMatch);
                _matches.Add(newMatch);
                player.CurrentMatch = newMatch;
                return true;
            }
            return false;
        }

        public bool JoinGame(string matchID, PlayerController player)
        {
            if (_matchIDs.Contains(matchID))
            {
                for (int i = 0; i < _matches.Count; i++)
                {
                    if (_matches[i].ID == matchID)
                    {
                        if(!_matches[i].InMatch && !_matches[i].MatchFull)
                        {
                            _matches[i].Players.Add(player);
                            player.CurrentMatch = _matches[i];
                            _matches[i].Players[0].PlayerCountUpdate(_matches[i].Players.Count);
                            if (_matches[i].Players.Count == _maxPlayers)
                                _matches[i].MatchFull = true;
                            break;
                        }
                        else
                            return false;
                    }
                }

                return true;
            }
            return false;
        }

        public bool SearchGame(PlayerController player, out string ID)
        {
            ID = "";

            for (int i = 0; i < _matches.Count; i++)
            {
                if (!_matches[i].InMatch && !_matches[i].MatchFull && _matches[i].PublicMatch)
                {
                    if (JoinGame(_matches[i].ID, player))
                    {
                        ID = _matches[i].ID;
                        return true;
                    }
                }
            }

            return false;
        }

        public PlayerData SpawnPlayerPrefab(PlayerController player)
        {
            PlayerData newPlayer = Instantiate(_playerDataPrefab, _playerDataHolder);
            newPlayer.SetPlayer(player);
            return newPlayer;
        }

        public void BeginGame(string matchID)
        {
            for (int i = 0; i < _matches.Count; i++)
            {
                if (_matches[i].ID == matchID)
                {
                    _matches[i].InMatch = true;
                    foreach (PlayerController player in _matches[i].Players)
                    {
                        player.StartGame();
                    }
                    break;
                }
            }
        }

        public void PlayerDisconnect(PlayerController player, string ID)
        {
            for (int i = 0; i < _matches.Count; i++)
            {
                if (_matches[i].ID == ID)
                {
                    int playerIndex = _matches[i].Players.IndexOf(player);
                    if (_matches[i].Players.Count > playerIndex)
                    {
                        _matches[i].Players.RemoveAt(playerIndex);
                    }

                    if (_matches[i].Players.Count == 0)
                    {
                        _matches.RemoveAt(i);
                        _matchIDs.Remove(ID);
                    }
                    else
                    {
                        _matches[i].Players[0].PlayerCountUpdate(_matches[i].Players.Count);
                    }
                    break;
                }
            }
        }

        public void SendMessageToServer(string message)
        {
            _chatHistoryText.text += message + "\n";
            _messageInput.text = string.Empty;
        }
    }
}