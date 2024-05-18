using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;
using DG.Tweening;
using System.Linq;

public class ShopWindowUI : NetworkBehaviour
{
    [SerializeField] private Button _closeBtn;

    private CanvasGroup _canvasGroup;
    public bool IsOpen => _canvasGroup.interactable;

    [HideInInspector] public PlayerController customer;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _closeBtn.onClick.AddListener(() => ActiveWindow(false));

        GetComponentsInChildren<UpgradeShopItem>().ToList().ForEach(shop => shop.Initialize(this));
    }

    public void ActiveWindow(bool value)
    {
        _canvasGroup.interactable = value;
        _canvasGroup.blocksRaycasts = value;

        float alpha = value ? 1 : 0;
        _canvasGroup.DOFade(alpha, 0.4f);
    }

    public void SetCustomer(PlayerController player)
    {
        customer = player;
    }
}
