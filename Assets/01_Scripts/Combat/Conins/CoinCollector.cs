using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class CoinCollector : NetworkBehaviour
{
    [Header("Reference Variable")]
    [SerializeField] private BountyCoin _coinPrefab;
    [SerializeField] private float _bountyRatio;
    [SerializeField] private int _minCoinValue = 50;
    private Health _health;

    public NetworkVariable<int> totalCoin;

    private void Awake()
    {
        totalCoin = new NetworkVariable<int>();
        _health = GetComponent<Health>();
    }


    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _health.OnDieEvent += HandleDieEvent;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        _health.OnDieEvent -= HandleDieEvent;
    }

    private void HandleDieEvent()
    {


        if (totalCoin.Value < _minCoinValue) return;


        int bountyValue = Mathf.FloorToInt(totalCoin.Value * _bountyRatio);

        float coinScale = Mathf.Clamp(bountyValue / 100f, 1f, 3f);


        BountyCoin newCoin = Instantiate(_coinPrefab, transform.position, Quaternion.identity);

        newCoin.SetValue(bountyValue);
        newCoin.NetworkObject.Spawn();
        newCoin.SetScaleAndVisible(coinScale);
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

    public void SpendCoin(int Value)
    {
        totalCoin.Value -= Value;
    }
}
