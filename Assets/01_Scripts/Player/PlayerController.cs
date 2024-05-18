using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Cinemachine;
using TMPro;
using Unity.Collections;

public class PlayerController : NetworkBehaviour
{
    public static event Action<PlayerController> OnPlayerSpawn;
    public static event Action<PlayerController> OnPlayerDespawn;

    [Header("Reference")]
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] private SpriteRenderer _minimapIcon;

    [Header("Setting Values")]
    [SerializeField] private int _ownerCamPriority = 15;
    [SerializeField] private Color _ownerColor;


    public NetworkVariable<Color> tankColor;

    public PlayerVisual VisualCompo { get; private set; }
    public PlayerMovement MovementCompo { get; private set; }
    public Health HealthCompo { get; private set; }
    public CoinCollector CoinCompo { get; private set; }
    public ProjectileLauncher LauncherCompo { get; private set; }


    public NetworkVariable<FixedString32Bytes> playerName;

    public ShopNPC shopNpc;
    private PlayerInput _playerInput;

    private void Awake()
    {
        tankColor = new NetworkVariable<Color>();
        playerName = new NetworkVariable<FixedString32Bytes>();

        VisualCompo = GetComponent<PlayerVisual>();
        MovementCompo = GetComponent<PlayerMovement>();
        HealthCompo = GetComponent<Health>();


        CoinCompo = GetComponent<CoinCollector>();
        _playerInput = GetComponent<PlayerInput>();

        LauncherCompo = GetComponent<ProjectileLauncher>();
    }

    public override void OnNetworkSpawn()
    {
        tankColor.OnValueChanged += HandleColorChanged;
        playerName.OnValueChanged += HandleNameChaner;

        if (IsOwner)
        {
            _minimapIcon.color = _ownerColor;
            _followCam.Priority = _ownerCamPriority;
            _playerInput.OnShopKeyEvent += HandleShopKeyEvent;
        }

        if (IsServer)
        {
            UserData qwer = HostSingleton.Instance.GameManager.NetServer.GetUserDataByClientID(OwnerClientId);
            playerName.Value = qwer.username;


        }
        HandleNameChaner(string.Empty, playerName.Value);
        OnPlayerSpawn?.Invoke(this);
    }

    private void HandleNameChaner(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        _nameText.text = newValue.ToString();
    }

    public override void OnNetworkDespawn()
    {
        playerName.OnValueChanged -= HandleNameChaner;
        tankColor.OnValueChanged -= HandleColorChanged;

        if (IsOwner)
            _playerInput.OnShopKeyEvent -= HandleShopKeyEvent;
        OnPlayerDespawn?.Invoke(this);

    }
    private void HandleShopKeyEvent()
    {
        if (shopNpc == null) return; //가게에 있을 때만 수행한다.

        shopNpc.OpenShop(this);
        //뭔가 할거임
    }


    private void HandleColorChanged(Color previousValue, Color newValue)
    {
        VisualCompo.SetTintColor(newValue);
    }


    #region Only Server execution area

    public void SetTankData(Color color, int coin)
    {
        tankColor.Value = color;
        CoinCompo.totalCoin.Value = coin;
        Debug.Log(tankColor.Value);
    }

    #endregion

    [ClientRpc]
    public void AddDamageToLauncherClientRpc(int upgradeValue)
    {
        LauncherCompo.damage += upgradeValue;
    }

    [ClientRpc]
    public void AddHPToPlayerClientRpc(int value)
    {
        HealthCompo.maxHealth += value;
    }
}
