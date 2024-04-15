using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private TankSelectUI _selectUIPrefab;
    [SerializeField] private RectTransform _selectPanelTrm;

    [SerializeField] private TankSelectPanel _tankSelectPanel;
    private void Awake()
    {
        Instance = this;
        _selectPanelTrm.parent.GetComponent<TankSelectPanel>();
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
            SpawnTank(clientID, color);
        }
    }

    public async void SpawnTank(ulong clientID, Color color, float delay = 0)
    {
        if (delay > 0)
        {
            await Task.Delay(Mathf.CeilToInt(delay * 1000));
        }

        Vector3 position = TankSpawnPoint.GetRandomSpawnPos();

        PlayerController tank = Instantiate(_playerPrefab, position, Quaternion.identity);
        tank.NetworkObject.SpawnAsPlayerObject(clientID);
        tank.SetTankColor(color);
    }
}
