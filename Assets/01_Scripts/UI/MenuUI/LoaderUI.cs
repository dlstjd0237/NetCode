using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class LoaderUI : MonoBehaviour
{
    private static LoaderUI _instance;

    public static LoaderUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LoaderUI>();
            }
            if (_instance == null)
            {
                Debug.LogError($"There is no LoaderUI");
            }
            return _instance;
        }
    }
    //[SerializeField] private Image _loadImage;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        //_loadImage.raycastTarget = false;
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

    }
    public void Show(bool value)
    {
        float fadeValue = value ? 1.0f : 0f;
        _canvasGroup.DOFade(fadeValue, 0.4f);
        _canvasGroup.blocksRaycasts = value;
        _canvasGroup.interactable = value;
        StartCoroutine(QWER());
    }
    IEnumerator QWER()
    {
        Color rand;
        var a = new WaitForSeconds(0.2f);
        for (int j = 0; j < 20; j++)
        {
            _text.text = $"Loading";
            for (int i = 0; i < 3; i++)
            {
                rand = new Color(Random.Range(0, 2f), Random.Range(0, 2f), Random.Range(0, 2f));
                _text.color = rand;
                _text.text += ".";
                yield return a;
            }
        }

    }
}
