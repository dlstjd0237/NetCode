using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGSConnectPanel : MonoBehaviour
{
    [SerializeField] private Button _relayHostBtn;
    [SerializeField] private Button _enterLobbyBtn;

    private void Awake()
    {
        _relayHostBtn.onClick.AddListener(HandleRelayHostClick);
    }

    private async void HandleRelayHostClick()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }
}
