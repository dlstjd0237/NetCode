using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TankSelectUI : NetworkBehaviour
{
    [SerializeField] private Image _tankImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Button[] _colorButtons;
    [SerializeField] private Button _readyBtn;
    [SerializeField] private Image _statusImage;

    [SerializeField] private Sprite _readySprite, _notReadySprite, _readyBtnSprite, _notReadyBtnSprite;

    public bool isReady;
    public Color selectedColor;

    private TextMeshProUGUI _readyBtnText;
    private NetworkVariable<FixedString32Bytes> playerName;


    private void Awake()
    {
        _readyBtnText = _readyBtn.GetComponentInChildren<TextMeshProUGUI>();

        playerName = new NetworkVariable<FixedString32Bytes>();

        playerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        _nameText.text = newValue.ToString();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner == false) return;
        _readyBtn.onClick.AddListener(HandleReadyBtnClick);

        isReady = false;
        SetReadyStatusVisual();

        foreach (Button button in _colorButtons)
        {
            button.onClick.AddListener(() =>
            {
                SetTankColor(button.image.color);

            });
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner == false) return;

        _readyBtn.onClick.RemoveListener(HandleReadyBtnClick);

        foreach (Button button in _colorButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void SetTankColor(Color color)
    {
        selectedColor = color;
        _tankImage.color = selectedColor;
    }

    private void SetReadyStatusVisual()
    {
        if (isReady)
        {
            _statusImage.sprite = _readySprite;
            _readyBtn.image.sprite = _readyBtnSprite;
            _readyBtnText.text = "준비 완료";
        }
        else
        {
            _statusImage.sprite = _notReadyBtnSprite;
            _readyBtn.image.sprite = _notReadyBtnSprite;
            _readyBtnText.text = "준비";
        }
    }

    #region Only Onwer execution area
    private void HandleReadyBtnClick()
    {
        isReady = !isReady;
        SetReadyStatusVisual();
        //서버에게 내가 변경되었음을 알려줘야 한다.
        SetReadyClaimToServerRpc(isReady);
    }

    #endregion


    #region Only server execution area
    public void SetTankName(string name)
    {
        playerName.Value = name;
    }

    [ServerRpc]
    private void SetReadyClaimToServerRpc(bool value)
    {
        isReady = value;
        SetReadyClientRpc(isReady);
    }
    #endregion

    #region Only Client execution area
    [ClientRpc]
    private void SetReadyClientRpc(bool value)
    {
        if (IsOwner) return;
        isReady = value;
        SetReadyStatusVisual();
    }
    #endregion
}
