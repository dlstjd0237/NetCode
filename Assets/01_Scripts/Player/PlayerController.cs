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
    [Header("Reference")]
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private TextMeshPro _nameText;

    [Header("Setting Values")]
    [SerializeField] private int _ownerCamPriority = 15;

    public NetworkVariable<Color> tankColor;

    public PlayerVisual VisualCompo { get; private set; }
    public PlayerMovement MovementCompo { get; private set; }

    public NetworkVariable<FixedString32Bytes> playerName;

    private void Awake()
    {
        tankColor = new NetworkVariable<Color>();
        playerName = new NetworkVariable<FixedString32Bytes>();

        VisualCompo = GetComponent<PlayerVisual>();
        MovementCompo = GetComponent<PlayerMovement>();
    }

    public override void OnNetworkSpawn()
    {
        tankColor.OnValueChanged += HandleColorChanged;
        playerName.OnValueChanged += HandleNameChaner;

        if (IsOwner)
        {
            _followCam.Priority = _ownerCamPriority;
        }

        if (IsServer)
        {

            UserData qwer = HostSingleton.Instance.GameManager.NetServer.GetUserDataByClientID(OwnerClientId);
            playerName.Value = qwer.username;

        }
        HandleNameChaner(string.Empty, playerName.Value);
    }

    private void HandleNameChaner(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        _nameText.text = newValue.ToString();
    }

    public override void OnNetworkDespawn()
    {
        playerName.OnValueChanged -= HandleNameChaner;
        tankColor.OnValueChanged -= HandleColorChanged;
    }

    private void HandleColorChanged(Color previousValue, Color newValue)
    {
        VisualCompo.SetTintColor(newValue);
    }

    #region Only Server execution area

    public void SetTankColor(Color color)
    {
        tankColor.Value = color;
    }

    #endregion
}
