using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class LeaderBoardBehaviour : NetworkBehaviour
{
    [SerializeField] private RankData _rankDataPrefab;
    [SerializeField] private RectTransform _contentRect;

    private int _displayCount = 5;

    private NetworkList<LeaderBoardEntityState> _leaderBoards;

    private void Awake()
    {
        _leaderBoards = new NetworkList<LeaderBoardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            PlayerController[] players = FindObjectsOfType<PlayerController>();


            foreach (PlayerController p in players)
            {
                HandlePlayerSpawn(p);
            }
            PlayerController.OnPlayerSpawn += HandlePlayerSpawn;
            PlayerController.OnPlayerDespawn += HandlePlayerDeSpawn;
        }
    }


    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            PlayerController.OnPlayerSpawn -= HandlePlayerSpawn;
            PlayerController.OnPlayerDespawn -= HandlePlayerDeSpawn;
        }
    }


    private void HandlePlayerSpawn(PlayerController player)
    {
    }
    private void HandlePlayerDeSpawn(PlayerController player)
    {
    }



  
}
