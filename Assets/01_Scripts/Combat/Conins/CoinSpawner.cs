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
    [SerializeField] private int _maxCoins = 30; //최대 30개씩 생성
    [SerializeField] private int _coinValue = 10; // 코인당 10
    [SerializeField] private LayerMask _layerMask; //코인 생성하는 지역에 장애물이 있는지 검사
    [SerializeField] private float _spawnTerm = 30.0f;
    [SerializeField] private float _spawnRadius = 8.0f;
    [SerializeField] private List<Transform> _spawnPointList;

    private bool _isSpawning = false;
    private float _spawnTime = 0;
    private int _spawnCountTime = 5; //5초 카운트다운 하고 시작

    private float _coinRadius;

    private Stack<ReSpawnCoin> _coinPool = new Stack<ReSpawnCoin>(); //코인 풀
    private List<ReSpawnCoin> _activeCoinList = new List<ReSpawnCoin>(); //활성화된 코인

    //이녀석도 서버만 실행하는 코드이다.
    private ReSpawnCoin SpawnCoin()
    {
        if (IsServer == false) return null;

        ReSpawnCoin coin = Instantiate(_coinPrefab, Vector3.zero, Quaternion.identity);
        coin.SetValue(_coinValue);
        coin.GetComponent<NetworkObject>().Spawn(); //네트워크를 통해서 이녀석을 다 스폰한다.

        coin.OnCollected += HandleCoinCollected;

        return coin;
    }

    //이것도 서버만 할꺼야
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

        //이걸로 코인 크기 잰다.
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
        if (IsServer == false) return; //서버가 아니면 리턴

        //나중에 여기에 게임이 시작되었을 때만 코인이 생성되게 변셩해야 함

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
        Debug.Log($"{pointIdex} 번 지점에서 {sec}초후 {coinCount}개의 코인이 생성됩니다.");
    }

    [ClientRpc]
    private void DecalCircleCloseClientRpc()
    {
        _decalCircle.CloseCircle();
    }
}
