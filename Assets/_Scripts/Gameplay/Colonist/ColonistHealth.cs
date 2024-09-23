using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class ColonistHealth : MonoBehaviour, IDamageTaker
{
    [field: SerializeField, ReadOnly] public float Health { get; private set; }

    public float MaxHealth { get; private set; } = 100;

    [field: SerializeField] public float DownedThreshold { get; private set; } = 15f;
    [field: SerializeField] public float HurtThreshold { get; private set; } = 80f;

    public event Action OnDeath;
    public event Action OnDowned;
    public event Action OnRecovered;
    public event Action<float, HealthStatus> OnHealthChanged;

    public HealthStatus Status { get; private set; }
    bool downed;
    void Awake()
    {
        Health = MaxHealth;
    }

    public bool IsDead()
    {
        return Health <= 0;
    }

    public bool IsDowned()
    {
        downed = Health < DownedThreshold;
        return downed;
    }

    [Button]
    public void TakeDamage(float damage)
    {
        damage = Mathf.Abs(damage);
        Health -= damage;
        Health = Mathf.Clamp(Health, 0, MaxHealth);
        UpdateHealth();
        if (IsDead())
        {
            Die();
        }
        else if (IsDowned())
        {
            GetDowned();
        }
    }

    public void UpdateHealth()
    {
        Status = GetStatus(Health);
        OnHealthChanged?.Invoke(Health, Status);
    }

    [Button]
    public void Heal(float healAmount)
    {
        healAmount = Mathf.Abs(healAmount);
        Health += healAmount;
        Health = Mathf.Clamp(Health, 0, MaxHealth);
        UpdateHealth();
        if (downed && !IsDowned())
        {
            OnRecovered?.Invoke();
        }
    }

    HealthStatus GetStatus(float health)
    {

        if (IsDead()) return HealthStatus.Dead;
        else if (IsDowned()) return HealthStatus.Downed;
        else if (health <= HurtThreshold) return HealthStatus.Hurt;
        else return HealthStatus.Healthy;
    }

    void GetDowned()
    {
        OnDowned?.Invoke();
    }

    void Die()
    {
        OnDeath?.Invoke();
    }
}