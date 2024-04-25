using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class NetworkConnection : MonoBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private Button _startHost;
        [SerializeField] private Button _startClient;

        private void Start()
        {
            if (!Application.isBatchMode)
            {
                _networkManager.StartClient();
            }
        }

        private void Awake()
        {
            Bind();
        }

        private void OnDestroy()
        {
            Expose();
        }

        private void Bind()
        {
            _startHost.onClick.AddListener(StartHost);
            _startClient.onClick.AddListener(StartClient);
        }

        private void Expose()
        {
            _startHost.onClick.RemoveListener(StartHost);
            _startClient.onClick.RemoveListener(StartClient);
        }

        private void StartHost()
        {
            _networkManager.StartHost();
        }

        private void StartClient()
        {
            _networkManager.networkAddress = "localhost";
            _networkManager.StartClient();
        }
    }
}