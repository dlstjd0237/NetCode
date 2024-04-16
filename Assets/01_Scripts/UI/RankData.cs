using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RankData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmpTextName;

    public RectTransform Rect { get; private set; }
    private string _playerName;

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

        UpdateCoin(0);
    }

    private void UpdateCoin(int v)
    {
        Coins = v;
        UpdateText();
    }

    private void UpdateText()
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            _tmpTextName.color = new Color(0.8f, 0.2f, 0.2f);
        }
        Text = $"{rank}, {_playerName}-{Coins}";
    }

    public void Show(bool value)
    {
        gameObject.SetActive(value);
    }
}
