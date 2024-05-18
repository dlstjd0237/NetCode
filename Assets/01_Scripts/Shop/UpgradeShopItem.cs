using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public abstract class UpgradeShopItem : NetworkBehaviour
{
    [SerializeField] protected Button _purchaseBtn;
    [SerializeField] protected int _upgradeCost = 100;
    [SerializeField] protected TextMeshProUGUI _purchaseText;
        
    protected ShopWindowUI _parent;

    protected virtual void Start()
    {
        _purchaseText.text = $"{_upgradeCost} G";
        _purchaseBtn.onClick.AddListener(HandlePurchaseClick);
    }

    public void Initialize(ShopWindowUI parent)
    {
        _parent = parent;
    }

    protected void HandlePurchaseClick()
    {
        if (_parent.customer == null) return; //손님이 없으면 구매도 없음.
        if (_parent.customer.CoinCompo.totalCoin.Value < _upgradeCost)
        {
            MessageSystem.Instance.ShowText("Not enough coin", 0.8f);
            return;
        }

        PurchaseProcess();
    }

    protected abstract void PurchaseProcess();
}