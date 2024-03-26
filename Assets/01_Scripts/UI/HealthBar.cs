using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform _barTrm;
    [SerializeField] private Health _ownerHealth;

    private void Awake()
    {
        _ownerHealth.OnHealthChangedEvent += HandleHealthChangedEvent;
    }

    private void HandleHealthChangedEvent()
    {
        float ratio = _ownerHealth.GetNormalizedHealth();
        _barTrm.localScale = new Vector3(ratio, 1, 1);
    }
}
