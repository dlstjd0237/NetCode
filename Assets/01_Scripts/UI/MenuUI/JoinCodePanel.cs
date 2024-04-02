using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

public class JoinCodePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _joinCodeText;
    [SerializeField] private Button _joinBtn;
    [SerializeField] private TextMeshProUGUI _joinCode;
    string joinCode;
    private void Awake()
    {

        _joinBtn.onClick.AddListener(HandleJointBtnClick);
    }



    private async void HandleJointBtnClick()
    {
        joinCode = _joinCodeText.text;
        await ClientSingleton.Instance.GameManager.StartClientWithJoinCode(joinCode);
    }
}
