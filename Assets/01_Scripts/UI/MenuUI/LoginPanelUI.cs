using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_InputField _inputName;
    [SerializeField] private Button _btnLogin;

    private void Awake()
    {
        _btnLogin.interactable = false;
        _btnLogin.onClick.AddListener(HandleLoginBtnClick);
        _inputName.onValueChanged.AddListener(ValidateUsername);
    }

    private void HandleLoginBtnClick()
    {
        ClientSingleton.Instance.GameManager.SetPlayerName(_inputName.text);
        _canvasGroup.DOFade(0, 0.3f).OnComplete(() =>
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        });
    }

    private void ValidateUsername(string name)
    {
        Regex regex = new Regex(@"[a-zA-Z0-9]{3,7}");
        bool success = regex.IsMatch(name);
        _btnLogin.interactable = success;
    }
}