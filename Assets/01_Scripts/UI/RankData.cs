using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RankData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmpTextName;

    public RectTransform Rect { get; private set; }
    private string _playerName;

    private PlayerController _targetPlayer;
    private Color _textColor = Color.white;

    public ulong clientID { get; private set; }
    public int Coins { get; private set; }
    public int rank;
    public string Text
    {
        get => _tmpTextName.text;
        set => _tmpTextName.text = value;

    }

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
    }

    public void Initialize(LeaderBoardEntityState state)
    {
        _playerName = state.playerName.Value;
        clientID = state.clientID;

        LoadTankDataAsync();

        UpdateCoin(0);
    }

    private async void LoadTankDataAsync()
    {
        while (_targetPlayer == null)
        {
            await Task.Delay(100);
            _targetPlayer = GameManager.Instance.GetPlayerByClientID(clientID);

        }
        _targetPlayer.tankColor.OnValueChanged += HandleColorChange;
        HandleColorChange(_textColor, _targetPlayer.tankColor.Value);

    }

    private void OnDestroy()
    {
        if (_targetPlayer != null)
        {
            _targetPlayer.tankColor.OnValueChanged -= HandleColorChange;
        }
    }

    private void HandleColorChange(Color previousValue, Color newValue)
    {
        _textColor = newValue;
        UpdateText();
    }



    public void UpdateCoin(int coins)
    {
        Coins = coins;
        UpdateText();
    }

    public void UpdateText()
    {

        _tmpTextName.color = _textColor;

        Text = $"{rank}, {_playerName}-{Coins}";
    }

    public void Show(bool value)
    {
        gameObject.SetActive(value);
    }
}
