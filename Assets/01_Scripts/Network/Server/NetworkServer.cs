using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    public NetworkManager _networkManager;

    //Ŭ���̾�Ʈ ���̵�� Auth ���̵� �˾Ƴ��� ��
    private Dictionary<ulong, string> _clientToAuthDictionary = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> _authToUserDictionary = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager manager)
    {
        _networkManager = manager;
        _networkManager.ConnectionApprovalCallback += HandleApprovalCheck;

        _networkManager.OnServerStarted += HandleServerStart;
    }

    private void HandleServerStart()
    {
        _networkManager.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    //clientID = ������ ������� 0,1,2,3,4,5 �ణ Enum����
    //authID = ���ڿ� ����
    //���� ������ 2���� ��ųʸ� ��� ����
    private void HandleClientDisconnect(ulong clientID)
    {
        if (_clientToAuthDictionary.TryGetValue(clientID, out string authID))
        {
            _clientToAuthDictionary.Remove(clientID);
            _authToUserDictionary.Remove(authID);
        }
    }

    private void HandleApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        //��������
        string json = Encoding.UTF8.GetString(request.Payload);
        UserData data = JsonUtility.FromJson<UserData>(json);

        _clientToAuthDictionary[request.ClientNetworkId] = data.userAuthID;
        _authToUserDictionary[data.userAuthID] = data;

        response.CreatePlayerObject = false;
        response.Approved = true;

        HostSingleton.Instance.StartCoroutine(CreatePanelWithDelay(0.5f, request.ClientNetworkId, data.username));

    }

    private IEnumerator CreatePanelWithDelay(float time, ulong clientID, string userName)
    {
        yield return new WaitForSeconds(time);
        GameManager.Instance.CreateUIPanel(clientID, userName);
    }

    public void Dispose()
    {
        if (_networkManager == null) return;

        _networkManager.ConnectionApprovalCallback -= HandleApprovalCheck;
        _networkManager.OnServerStarted -= HandleServerStart;
        _networkManager.OnClientDisconnectCallback -= HandleClientDisconnect;

        if (_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }
}