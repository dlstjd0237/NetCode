using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class Health : NetworkBehaviour
{
    public NetworkVariable<int> currentHealth;
    public int maxHealth;

    public event Action OnDieEvent;
    public event Action OnHealthChangedEvent;

    private bool _isDead;
    [SerializeField] private GameObject _explosionParticle;

    private void Awake()
    {
        currentHealth = new NetworkVariable<int>();
    }

    public override void OnNetworkSpawn()
    {
        //주의사항2. NetworkVariable은 서버만 건드릴 수 있다. 클라는 값의 변경을 받기만
        if (IsClient)
        {
            currentHealth.OnValueChanged += HandleHealthValueChanged;
        }

        if (IsServer == false) return; //서버가 아님 저리 가잇!

        currentHealth.Value = maxHealth; // 처음시작시 최대체력으로 넣어준다.
    }

    public override void OnNetworkDespawn()
    {
        for (int i = 0; i < 4; ++i)
        {
            Vector2 pos = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle;
            Instantiate(_explosionParticle, pos, Quaternion.identity);
        }

        if (IsClient)
        {
            currentHealth.OnValueChanged -= HandleHealthValueChanged;
        }
    }

    public float GetNormalizedHealth()
    {
        return (float)currentHealth.Value / maxHealth;
    }

    private void HandleHealthValueChanged(int previousValue, int newValue)
    {
        OnHealthChangedEvent?.Invoke();

        int delta = newValue - previousValue;
        int value = Mathf.Abs(delta);

        if (value == maxHealth) return;

        Color textColor = delta < 0 ? Color.red : Color.green;
        TextManager.Instacnce.PopUpText(value.ToString(), transform.position, textColor);

    }

    //코이츠와 서버만 실행하는 메서드이다.
    private void ModifyHealth(int value)
    {
        if (_isDead) return;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value + value, 0, maxHealth);
        if (currentHealth.Value == 0)
        {
            OnDieEvent?.Invoke();
            _isDead = true;
        }

    }

    public void TakeDamage(int damageValue)
    {

        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }
}
