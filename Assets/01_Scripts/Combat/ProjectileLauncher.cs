using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.Events;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Transform _firePosTrm;
    [SerializeField] private GameObject _serverProjectilePrefab;
    [SerializeField] private GameObject _clientProjectilePrefab;

    [SerializeField] private Collider2D _playerCollider;

    [Header("Setting Values")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireCooltime;

    private bool _shouldFire;
    private float _prevFireTime;

    public UnityEvent OnFireEvent;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _playerInput.OnFireEvent += HandleFireEvent;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        _playerInput.OnFireEvent -= HandleFireEvent;
    }

    private void HandleFireEvent()
    {
        if (Time.time < _prevFireTime + _fireCooltime) return;

        FireServerRpc(_firePosTrm.position, _firePosTrm.up);
        SpawnDummyProjectile(_firePosTrm.position, _firePosTrm.up);
        _prevFireTime = Time.time;
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 dir)
    {
        GameObject projectile = Instantiate(_clientProjectilePrefab, spawnPos, Quaternion.identity);

        projectile.transform.up = dir; //위방향으로 회전

        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(_playerCollider, projectileCollider);

        Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
        rigidbody.velocity = projectile.transform.up * _projectileSpeed;

        OnFireEvent?.Invoke();
    }

    [ServerRpc]
    private void FireServerRpc(Vector3 spawnPos, Vector3 dir)
    {
        GameObject projectile = Instantiate(_serverProjectilePrefab, spawnPos, Quaternion.identity);
        projectile.transform.up = dir; //위방향으로 회전

        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(_playerCollider, projectileCollider);

        Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
        rigidbody.velocity = projectile.transform.up * _projectileSpeed;

        //만들어라잇
        SpawnDummyProjectileClientRpc(spawnPos, dir);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 dir)
    {
        if (IsOwner) return;

   
        SpawnDummyProjectile(spawnPos, dir);
    }

}
