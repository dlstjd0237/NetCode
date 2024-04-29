using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BountyCoin : Coin
{
    private CinemachineImpulseSource _impulseSource;

    protected override void Awake()
    {
        base.Awake();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    public override int Collect()
    {
        if (!IsServer)
        {
            SetVisible(false);
            return 0;
        }

        if (_alreadyCollected) return 0;
        _alreadyCollected = true;

        Destroy(gameObject); //������ ��Ʈ��ũ ������Ʈ �޷��ִ� �༮�� �ǰ��� 
        //��� Ŭ���׾�Ʈ���� ������� �μ�����.
        return _coinValue; ;
    }

    internal void SetScaleAndVisible(float coinScale)
    {
        isActive.Value = true;
        CoinSpawnClientRpc(coinScale);
    }

    [ClientRpc]
    private void CoinSpawnClientRpc(float coinScale)
    {
        Vector3 destination = transform.position;
        transform.position = destination + new Vector3(0, 3f, 0);
        transform.localPosition = Vector3.one * coinScale;
        SetVisible(true);

        transform.DOMove(destination, 0.6f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            _impulseSource.GenerateImpulse(0.3f);
        });
    }
}
