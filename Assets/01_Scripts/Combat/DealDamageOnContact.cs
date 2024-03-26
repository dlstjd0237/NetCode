using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int _damage = 10;

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody is null) return;//리지드 바디가 안붙어 있다면.

        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(_damage);
        }
    }
}
