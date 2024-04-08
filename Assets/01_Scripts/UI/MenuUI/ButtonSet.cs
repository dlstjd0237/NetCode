using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ButtonSet : MonoBehaviour
{
    [SerializeField] private Image _lobbyPanel;
    [SerializeField] private Button _hostBtn, _exitBtn;
    [SerializeField] private Vector2 _movePos;
    private Vector2 _defualt;

    private void Awake()
    {
        _defualt = _lobbyPanel.rectTransform.position;
        _hostBtn.onClick.AddListener(HostSet);
        _exitBtn.onClick.AddListener(ExitSet);
    }

    private void ExitSet()
    {
        _lobbyPanel.rectTransform.DOMoveY(_defualt.y, 2);
        _lobbyPanel.DOFade(0, 2);
    }

    private void HostSet()
    {
        _lobbyPanel.rectTransform.DOMoveY(_movePos.y, 2);
        _lobbyPanel.DOFade(1, 2);
    }
}
