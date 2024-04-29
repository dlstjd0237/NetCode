using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private PlayerController _playerPrefab;
    public PlayerController PlayerPrefab => _playerPrefab;
    [SerializeField] private TankSelectUI _selectUIPrefab;
    [SerializeField] private RectTransform _selectPanelTrm;

    [SerializeField] private TankSelectPanel _tankSelectPanel;

    private Dictionary<ulong, PlayerController> _playerDictionary;
    private bool _isGameStart = false;
    public bool GameStarted => _isGameStart;
    private void Awake()
    {
        Instance = this;
        _selectPanelTrm.parent.GetComponent<TankSelectPanel>();
        _playerDictionary = new Dictionary<ulong, PlayerController>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            PlayerController.OnPlayerSpawn += HandlePlayerSpawn;
            PlayerController.OnPlayerDespawn += HandlePlayerDeSpawn;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            PlayerController.OnPlayerSpawn -= HandlePlayerSpawn;
            PlayerController.OnPlayerDespawn -= HandlePlayerDeSpawn;
        }
    }

    private void HandlePlayerSpawn(PlayerController controller)
    {
        _playerDictionary.Add(controller.OwnerClientId, controller);
    }

    private void HandlePlayerDeSpawn(PlayerController controller)
    {
        if (_playerDictionary.ContainsKey(controller.OwnerClientId))
        {
            _playerDictionary.Remove(controller.OwnerClientId);
        }
    }

    public PlayerController GetPlayerByClientID(ulong clientID)
    {
        if (_playerDictionary.TryGetValue(clientID, out PlayerController controller))
        {
            return controller;
        }
        return null;
    }


    public void CreateUIPanel(ulong clientID, string username)
    {
        TankSelectUI ui = Instantiate(_selectUIPrefab);
        ui.NetworkObject.SpawnAsPlayerObject(clientID);
        ui.transform.SetParent(_selectPanelTrm);
        ui.transform.localScale = Vector3.one;

        _tankSelectPanel.AddSelectUI(ui);//이건 호스트만 실행하니까

        ui.SetTankName(username);
    }

    public void StartGame(List<TankSelectUI> tankUIList)
    {
        foreach (TankSelectUI ui in tankUIList)
        {
            ulong clientID = ui.OwnerClientId;//이걸 소유하고 있는 유저의 clientID
            Color color = ui.selectedColor.Value;
            SpawnTank(clientID, color, 0);
        }
        _isGameStart = true;
    }



    public void SpawnTank(ulong clientID, Color color, int coin, float delay = 0)
    {
        StartCoroutine(DelayedSpawn(clientID, color, coin, delay));
    }


    private IEnumerator DelayedSpawn(ulong clientID, Color color, int coin, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        Vector3 position = TankSpawnPoint.GetRandomSpawnPos();

        PlayerController tank = Instantiate(_playerPrefab, position, Quaternion.identity);
        tank.NetworkObject.SpawnAsPlayerObject(clientID);
        tank.SetTankData(color, coin);
    }


}
