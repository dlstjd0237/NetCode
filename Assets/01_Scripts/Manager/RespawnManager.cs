using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class RespawnManager : NetworkBehaviour
{
    [SerializeField] private float _keepCoinRatio = 0.2f;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        foreach (PlayerController player in players)
        {
            HandlePlayerSpawn(player);
        }

        PlayerController.OnPlayerSpawn += HandlePlayerSpawn;
        PlayerController.OnPlayerDespawn += HandlePlayerDespawn;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        PlayerController.OnPlayerSpawn -= HandlePlayerSpawn;
        PlayerController.OnPlayerDespawn -= HandlePlayerDespawn;
    }

    private void HandlePlayerSpawn(PlayerController player)
    {
        player.HealthCompo.OnDieEvent += () =>
        {
            ulong clientID = player.OwnerClientId;
            Color color = player.tankColor.Value;

            int remainCoin = Mathf.FloorToInt(player.CoinCompo.totalCoin.Value * _keepCoinRatio);

            Destroy(player.gameObject);
            //클라아이디, 컬러, 딜레이
            GameManager.Instance.SpawnTank(clientID, color,remainCoin, 10.0f);

        };
    }

    private void HandlePlayerDespawn(PlayerController player)
    {

    }
}
