using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class CoinCollector : NetworkBehaviour
{
    public NetworkVariable<int> totalCoin;

    private void Awake()
    {
        totalCoin = new NetworkVariable<int>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Coin>(out Coin coin))
        {
            int value = coin.Collect();

            if (!IsServer) return;
            totalCoin.Value += value;
        }
    }
}
