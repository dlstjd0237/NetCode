using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TankSelectPanel : NetworkBehaviour
{
    [SerializeField] private Button _startBtn;
    [SerializeField] private List<TankSelectUI> _selectUIList;
    private void Awake()
    {
        _selectUIList = new List<TankSelectUI>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            _startBtn.onClick.AddListener(HandleGameStart);
        }
        else
        {
            _startBtn.gameObject.SetActive(false);
        }
    }

    private void HandleGameStart()
    {
        GameManager.Instance.StartGame(_selectUIList);
        GameStartClientRpc();
    }

    [ClientRpc]
    public void GameStartClientRpc()
    {
        gameObject.SetActive(false);
    }

    public void AddSelectUI(TankSelectUI ui)
    {
        _selectUIList.Add(ui);
        ui.OnDisconnectEvent += HandleDisconnected;
        ui.OnReadyChangeEvent += HandleReadyChanged;
        HandleReadyChanged(false);

    }
    private void HandleDisconnected(TankSelectUI ui)
    {
        ui.OnDisconnectEvent -= HandleDisconnected;
        _selectUIList.Remove(ui);
    }
    private void HandleReadyChanged(bool value)
    {
        bool allReady = _selectUIList.Count > 0 && _selectUIList.Any(x => x.isReady.Value == false) == false;

        _startBtn.interactable = allReady;

    }




}
