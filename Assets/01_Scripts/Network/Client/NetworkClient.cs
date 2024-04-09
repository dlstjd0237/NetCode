using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager _networkManager;

    public NetworkClient(NetworkManager manager)
    {
        _networkManager = manager;
        _networkManager.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    //서버의 클라이언트 ID는 무조건 0
    private void HandleClientDisconnect(ulong clientID)
    {
        //서버가 나갔거나, 작가 나간것'
        if (clientID != 0 && clientID != _networkManager.LocalClientId) return;

        Disconnect();
    }

    public void Disconnect()
    {
        if (SceneManager.GetActiveScene().name != SceneNames.MenuScene)
        {
            SceneManager.LoadScene(SceneNames.MenuScene);
        }

        if (_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown();//강제 종료
        }
    }

    public void Dispose()
    {
        if (_networkManager != null)
        {
            _networkManager.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }
}
