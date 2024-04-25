using LobbySystem;
using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PlayerSystem
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private NetworkMatch _networkMatch;
        [SerializeField] private TextMeshProUGUI _nameText;

        private InputListener _inputListener;
        private PlayerData _playerData;
        private MainMenu _mainMenu;
        private Guid _netIDGuid;

        [SyncVar] public Match CurrentMatch;
        [SyncVar] private string _matchID;
        [SyncVar] private string _name;

        public static PlayerController LocalPlayer;

        [Inject]
        public void Construct(InputListener inputListener, MainMenu mainMenu)
        {
            _inputListener = inputListener;
            _mainMenu = mainMenu;
            Bind();
        }

        private void Start()
        {
            _nameText.text = _name;
        }

        private void OnDestroy()
        {
            Expose();
        }
        public override void OnStartServer()
        {
            _netIDGuid = netId.ToString().ToGuid();
            _networkMatch.matchId = _netIDGuid;
        }

        public override void OnStartClient()
        {
            if (isLocalPlayer)
                LocalPlayer = this;
            else
                _playerData = _mainMenu.SpawnPlayerPrefab(this);
        }

        public override void OnStopClient()
        {
            ClientDisconnect();
        }

        public override void OnStopServer()
        {
            ServerDisconnect();
        }

        private void Bind()
        {
            _inputListener.AddListener(this);
        }

        private void Expose()
        {
            _inputListener.RemoveListener(this);
        }

        [Command]
        private void CmdHostGame(string ID, string name, bool publicMatch)
        {
            _matchID = ID;
            _name = name;
            _nameText.text = name;

            if (_mainMenu.HostGame(ID, this, publicMatch))
            {
                _networkMatch.matchId = ID.ToGuid();
                TargetHostGame(true, ID, name);
            }
            else
            {
                TargetHostGame(false, ID, name);
            }
        }

        [TargetRpc]
        private void TargetHostGame(bool success, string ID, string name)
        {
            _matchID = ID;
            _name = name;
            _nameText.text = name;

            _mainMenu.CheckSuccess(success, ID, true);
        }

        [Command]
        private void CmdJoinGame(string ID, string name)
        {
            _matchID = ID;
            _name = name;
            _nameText.text = name;

            if (_mainMenu.JoinGame(ID, this))
            {
                _networkMatch.matchId = ID.ToGuid();
                TargetJoinGame(true, ID, name);
            }
            else
            {
                TargetJoinGame(false, ID, name);
            }
        }

        [TargetRpc]
        private void TargetJoinGame(bool success, string ID, string name)
        {
            _matchID = ID;
            _name = name;
            _nameText.text = name;
            _mainMenu.CheckSuccess(success, ID, false);
        }

        [Command]
        private void CmdBeginGame()
        {
            _mainMenu.BeginGame(_matchID);
        }

        [TargetRpc]
        private void TargetBeginGame()
        {
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            for (int i = 0; i < players.Length; i++)
            {
                DontDestroyOnLoad(players[i]);
            }
            transform.localScale = new Vector3(1,1,1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        [Command]
        private void CmdDisconnectGame()
        {
            ServerDisconnect();
        }

        [Command]
        private void CmdSearchGame(string name)
        {
            _name = name;
            _nameText.text = name;
            if (_mainMenu.SearchGame(this, out _matchID))
            {
                _networkMatch.matchId = _matchID.ToGuid();
                TargetSearchGame(true, _matchID, name); 
                if (isServer && _playerData != null)
                {
                    _playerData.gameObject.SetActive(true);
                }
            }
            else
            {
                TargetSearchGame(false, _matchID, name);
            }
        }

        [TargetRpc]
        private void TargetSearchGame(bool success, string ID, string name)
        {
            _matchID = ID;
            _name = name;
            _nameText.text = name;
            _mainMenu.CheckSuccess(success, ID, false);
        }

        [Server]
        public void PlayerCountUpdate(int playerCount)
        {
            TargetPlayerCountUpdate(playerCount);
        }

        [TargetRpc]
        private void TargetPlayerCountUpdate(int playerCount)
        {
            if (playerCount > 1)
            {
                _mainMenu.SetBeginButtonActive(true);
            }
            else
            {
                _mainMenu.SetBeginButtonActive(false);
            }
        }

        [ClientRpc]
        private void RpcDisconnectGame()
        {
            ClientDisconnect();
        }

        [Command]
        public void CmdHandleMessage(string message)
        {
            RpcHandleMessage($"{_name}: {message}");
        }
        
        [ClientRpc]
        private void RpcHandleMessage(string message)
        {
            _mainMenu.SendMessageToServer(message);
        }

        private void ClientDisconnect()
        {
            if (_playerData != null)
            {
                if (!isServer)
                {
                    Destroy(_playerData.gameObject);
                    _playerData = null;
                }
                else
                {
                    _playerData.gameObject.SetActive(false);
                }
            }
        }

        private void ServerDisconnect()
        {
            _mainMenu.PlayerDisconnect(this, _matchID);
            RpcDisconnectGame();
            _networkMatch.matchId = _netIDGuid;
        }

        public void Move(float horizontal, float vertical)
        {
            transform.Translate(new Vector2(horizontal * _speed * Time.deltaTime,
                vertical * _speed * Time.deltaTime));
        }

        public void HostGame(string name, bool publicMatch)
        {
            string ID = RandomIdGenerator.GetRandomID();
            CmdHostGame(ID, name, publicMatch);
        }

        public void StartGame()
        {
            TargetBeginGame();
        }

        public void BeginGame()
        {
            CmdBeginGame();
        }

        public void JoinGame(string inputID, string name)
        {
            CmdJoinGame(inputID, name);
        }

        public void DisconnectGame()
        {
            CmdDisconnectGame();
        }

        public void SearchGame(string name)
        {
            CmdSearchGame(name);
        }

        public string GetName() => _name;
    }
}