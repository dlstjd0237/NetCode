using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using System;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class ClientGameManager : MonoBehaviour
{
    private JoinAllocation _allocation;
    private string _playerName;
    public string PlayerName => _playerName;
    public async Task<bool> InitAsync()
    {
        //여기에 UGS서비스 인증파트가 들어갈 예정입니다.
        await UnityServices.InitializeAsync(); //초기화

        AuthState authState = await UGSAuthWrapper.DoAuth(); //인증이 5회 진행될꺼임

        if (authState == AuthState.Authenticated)
        {
            return true;
        }
        return false;
    }

    public async Task StartClientWithJoinCode(string joinCode)
    {
        try
        {
            _allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            var relayServerData = new RelayServerData(_allocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }
    }

    public void GotoMenueScene()
    {
        SceneManager.LoadScene(SceneNames.MenuScene);
    }

    public bool StartClinetLocalNetwork()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            return true;
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
            return false;
        }
    }

    public void SetPlayerName(string playerName)
    {
        _playerName = playerName;
    }
}
