using System.Net.Sockets;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using static ConnectionNotificationManager;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    private NetworkVariable<int> numberOfPlayer = new(0);

    public override void OnNetworkSpawn()
    {
        if (IsHost || IsServer)
        {
            StartGameServerRpc();
        }
    }

    private void Awake()
    {
        string myAddressLocal = "";
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                myAddressLocal = ip.ToString();
                break;
            }
        }
        if (myAddressLocal == "") myAddressLocal = "127.0.0.1";

        serverBtn.onClick.AddListener(() =>
        {
            UnityTransport transport = (UnityTransport)NetworkManager.NetworkConfig.NetworkTransport;
            transport.SetConnectionData(myAddressLocal, 7777);
            NetworkManager.Singleton.StartServer();
        });
        hostBtn.onClick.AddListener(() =>
        {
            UnityTransport transport = (UnityTransport)NetworkManager.NetworkConfig.NetworkTransport;
            transport.SetConnectionData(myAddressLocal, 7777);
            NetworkManager.Singleton.StartHost();
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

    private void Start()
    {
        Singleton.OnClientConnectionNotification += PlayerJoin;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Singleton.OnClientConnectionNotification -= PlayerJoin;
    }

    private void PlayerJoin(ulong clientId, ConnectionStatus con)
    {
        if (con == ConnectionStatus.Connected)
            numberOfPlayer.Value += 1;
        else
            numberOfPlayer.Value -= 1;
    }

    private void StartGame()
    {
        CountdownUIHandler countdownUIHandler = FindAnyObjectByType<CountdownUIHandler>();
        if (countdownUIHandler != null)
        {
            countdownUIHandler.StartCountDown();
        }
    }

    [ServerRpc]
    private void StartGameServerRpc()
    {
        //numberOfPlayer.OnValueChanged += (int prevValue, int newValue) =>
        //{
        //    if (numberOfPlayer.Value >= 2)
        //    {
                StartGame();
                Singleton.OnClientConnectionNotification -= PlayerJoin;
                StartGameClientRpc();
        //    }
        //    Debug.Log(numberOfPlayer);
        //};
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        StartGame();
    }
}
