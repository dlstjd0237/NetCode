using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerAniming : NetworkBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Transform _turretTrm;
    private Camera _mainCame;

    private void Awake()
    {
        _mainCame = Camera.main;
    }
    private void LateUpdate()
    {
        if (!IsOwner) return;

        Vector3 dir = (_mainCame.ScreenToWorldPoint(_playerInput.AimPosition) - transform.position).normalized;

        _turretTrm.up = new Vector2(dir.x, dir.y);
        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90.0f;
        //_turretTrm.rotation = Quaternion.Euler(0, 0, angle);
    }
}
