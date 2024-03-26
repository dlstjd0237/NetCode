using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MuzzelFlashFeedback : Feedback
{
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private Light2D _muzzleLight;
    [SerializeField] private float _turnOnTIme = 0.0f;
    [SerializeField] private bool _defaultState;

    public override void CompleteFeedback()
    {
        StopAllCoroutines();
        _muzzleFlash.SetActive(_defaultState);
        _muzzleLight.gameObject.SetActive(_defaultState);
    }

    public override void CreateFeedback()
    {
        StartCoroutine(ActiveCoroutine());
    }

    private IEnumerator ActiveCoroutine()
    {
        _muzzleLight.gameObject.SetActive(true);
        _muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(_turnOnTIme);
        _muzzleLight.gameObject.SetActive(false);
        _muzzleFlash.SetActive(false);
    }


}
