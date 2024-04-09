using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private Allocation _allocation;
    private const int _maxConnections = 8;
    private string _joinCode;
    public string JoinCode => _joinCode;
    private string _lobbyId; //���� ������ �����, �κ� ����ǵ� �� ���̵�

    public NetworkServer NetServer { get; private set; }
    private void MakeNetworkServer()
    {
        NetServer = new NetworkServer(NetworkManager.Singleton);
    }


    public async Task StartHostAsync()
    {
        try
        {
            _allocation = await Relay.Instance.CreateAllocationAsync(_maxConnections);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        try
        {
            _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log(_joinCode);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        //tcp, udp, dtcp, dtls
        RelayServerData relayData = new RelayServerData(_allocation, "dtls");
        transport.SetRelayServerData(relayData);

        //�κ� ������ ������ش�.
        try
        {
            CreateLobbyOptions lobbyOption = new CreateLobbyOptions();
            lobbyOption.IsPrivate = false;
            lobbyOption.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode",
                    new DataObject(DataObject.VisibilityOptions.Member, _joinCode)
                }

            };

            string playerName = ClientSingleton.Instance.GameManager.PlayerName;

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", _maxConnections, lobbyOption);

            _lobbyId = lobby.Id; //������� �κ��� ���̵� ����Ѵ�.

            //15�� �������� ��Ʈ��Ʈ �� ������
            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex);
            return;
        }

        MakeNetworkServer();

        ClientSingleton.Instance.GameManager.SetPayloadData();

        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneNames.GameScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    private IEnumerator HeartBeatLobby(float waitTime)
    {
        var timer = new WaitForSecondsRealtime(waitTime);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
            yield return timer;
        }


    }

    public bool StartHostLocalNetwork()
    {
        MakeNetworkServer();
        ClientSingleton.Instance.GameManager.SetPayloadData();
        //����� �ڵ� �Ű������� �ȴ�.
        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneNames.GameScene, LoadSceneMode.Single);
            return true;
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
            return false;
        }
    }

    public void Dispose()
    {
        Shutdown();
    }

    private async void Shutdown()
    {
        HostSingleton.Instance.StopAllCoroutines(); //��Ʈ��Ʈ ��������

        if (!string.IsNullOrEmpty(_lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogError(ex);
            }
        }

        _lobbyId = string.Empty;
        NetServer?.Dispose();
    }
}
