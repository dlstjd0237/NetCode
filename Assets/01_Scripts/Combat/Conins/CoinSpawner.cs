using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Random = UnityEngine.Random;
public class CoinSpawner : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private ReSpawnCoin _coinPrefab;
    [SerializeField] private DecalCircle _decalCircle;

    [Header("Setting values")]
    [SerializeField] private int _maxCoins = 30; //�ִ� 30���� ����
    [SerializeField] private int _coinValue = 10; // ���δ� 10
    [SerializeField] private LayerMask _layerMask; //���� �����ϴ� ������ ��ֹ��� �ִ��� �˻�
    [SerializeField] private float _spawnTerm = 30.0f;
    [SerializeField] private float _spawnRadius = 8.0f;
    [SerializeField] private List<Transform> _spawnPointList;

    private bool _isSpawning = false;
    private float _spawnTime = 0;
    private int _spawnCountTime = 5; //5�� ī��Ʈ�ٿ� �ϰ� ����

    private float _coinRadius;

    private Stack<ReSpawnCoin> _coinPool = new Stack<ReSpawnCoin>(); //���� Ǯ
    private List<ReSpawnCoin> _activeCoinList = new List<ReSpawnCoin>(); //Ȱ��ȭ�� ����

    //�̳༮�� ������ �����ϴ� �ڵ��̴�.
    private ReSpawnCoin SpawnCoin()
    {
        if (IsServer == false) return null;

        ReSpawnCoin coin = Instantiate(_coinPrefab, Vector3.zero, Quaternion.identity);
        coin.SetValue(_coinValue);
        coin.GetComponent<NetworkObject>().Spawn(); //��Ʈ��ũ�� ���ؼ� �̳༮�� �� �����Ѵ�.

        coin.OnCollected += HandleCoinCollected;

        return coin;
    }

    //�̰͵� ������ �Ҳ���
    private void HandleCoinCollected(ReSpawnCoin coin)
    {
        if (IsServer == false) return;

        _activeCoinList.Remove(coin);
        coin.SetVisible(false);
        _coinPool.Push(coin);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer == false)
        {
            return;
        }

        //�̰ɷ� ���� ũ�� ���.
        _coinRadius = _coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < _maxCoins; ++i)
        {
            ReSpawnCoin coin = SpawnCoin();
            coin.SetVisible(false);
            _coinPool.Push(coin);
        }
    }

    public override void OnNetworkDespawn()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (IsServer == false) return; //������ �ƴϸ� ����

        //���߿� ���⿡ ������ ���۵Ǿ��� ���� ������ �����ǰ� �����ؾ� ��

        if (_isSpawning == false && _activeCoinList.Count == 0)
        {
            _spawnTime += Time.deltaTime;
            if (_spawnTime >= _spawnTerm)
            {
                _spawnTime = 0;
                StartCoroutine(SpawnCoroutine());
            }
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        _isSpawning = true;
        int pointIndex = Random.Range(0, _spawnPointList.Count);
        int coinCount = Random.Range(_maxCoins / 2, _maxCoins + 1);

        for (int i = _spawnCountTime; i > 0; --i)
        {
            CountDownClientRpc(i, pointIndex, coinCount);
            yield return new WaitForSeconds(1.0f);
        }

        Vector2 center = _spawnPointList[pointIndex].position;

        float coinDelay = 3.0f;
        for (int i = 0; i < coinCount; ++i)
        {
            Vector2 pos = Random.insideUnitCircle * _spawnRadius + center;
            ReSpawnCoin coin = _coinPool.Pop();
            coin.transform.position = pos;
            coin.ResetCoin();
            _activeCoinList.Add(coin);
            yield return new WaitForSeconds(coinDelay);
        }
        _isSpawning = false;
        DecalCircleCloseClientRpc();
    }

    [ClientRpc]
    private void CountDownClientRpc(int sec, int pointIdex, int coinCount)
    {
        if (_decalCircle.showDecal == false)
        {
            _decalCircle.OpenCircle(_spawnPointList[pointIdex].position, _spawnRadius);
        }
        Debug.Log($"{pointIdex} �� �������� {sec}���� {coinCount}���� ������ �����˴ϴ�.");
    }

    [ClientRpc]
    private void DecalCircleCloseClientRpc()
    {
        _decalCircle.CloseCircle();
    }
}
