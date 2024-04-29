using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Transform _bodyTrm;
    [SerializeField] private ParticleSystem _dustEffect;
    private Rigidbody2D _rigdbody;

    [Header("Setting Values")]
    [SerializeField] private float _movementSpeed = 4.0f; //이동속도
    [SerializeField] private float _turningSpeed = 30.0f; //회전속도

    private Vector2 _movementInput;

    [SerializeField] private float _dustEmissionValue = 10;
    private ParticleSystem.EmissionModule _emissionModule;
    private Vector3 _prevPos;
    private float _particleStopThreshold = 0.005f;

    private void Awake()
    {
        _rigdbody = GetComponent<Rigidbody2D>();
        _emissionModule = _dustEffect.emission; //슈우우우우우우우웃


    }

    public override void OnNetworkSpawn() //누가 Woner인지 모르고 아무거나 다 움직실수 있음
    {
        if (!IsOwner) return;
        _playerInput.OnMovementEvent += HandleMovementEvent;
    }

    public override void OnNetworkDespawn()
    {
        _playerInput.OnMovementEvent -= HandleMovementEvent;
    }
    private void HandleMovementEvent(Vector2 obj)
    {
        _movementInput = obj;
    }

    private void Update()
    {
        if (!IsOwner) return;
        float zRot = _movementInput.x * -_turningSpeed * Time.deltaTime;
        transform.Rotate(0, 0, zRot);
    }
    private void FixedUpdate()
    {
        float moveDistance = (transform.position - _prevPos).sqrMagnitude;
        if (moveDistance > _particleStopThreshold)
        {
            _emissionModule.rateOverTime = _dustEmissionValue;
        }
        else
        {
            _emissionModule.rateOverTime = 0;
        }

        _prevPos = transform.position;

        if (!IsOwner) return;
        _rigdbody.velocity = _bodyTrm.up * (_movementInput.y * _movementSpeed);
    }
}
