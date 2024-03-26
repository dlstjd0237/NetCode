using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using DG.Tweening;
public abstract class Coin : NetworkBehaviour
{
    protected SpriteRenderer _spriteRenderer;
    protected CircleCollider2D _collider2D;
    protected int _coinValue = 10;
    protected bool _alreadyCollected;

    protected readonly int _viewOffsetHash = Shader.PropertyToID("_ViewOffset");

    protected Tween _viewTween = null;

    public NetworkVariable<bool> isActive;

    public abstract int Collect();

    protected void Awake()
    {
        isActive = new NetworkVariable<bool>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<CircleCollider2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            SetVisible(isActive.Value);
        }
    }

    public void SetVisible(bool value)
    {
        _collider2D.enabled = value;
        _spriteRenderer.enabled = value;

        if (value)
        {
            if (_viewTween != null && _viewTween.IsActive())
            {
                _viewTween.Kill();
            }
            Material mat = _spriteRenderer.material;
            mat.SetFloat(_viewOffsetHash, 0); //0���� ����
            float coinVisibleTime = 1.5f;

            _viewTween = DOTween.To(
                () => mat.GetFloat(_viewOffsetHash), value => mat.SetFloat(_viewOffsetHash, value), 1.1f, coinVisibleTime);
        }
    }

    public void SetValue(int value)
    {
        _coinValue = value;

    }
}
