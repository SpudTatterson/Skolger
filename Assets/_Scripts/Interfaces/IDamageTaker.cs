using System;
using UnityEngine;

public interface IDamageTaker
{
    public float Health { get; }
    public float MaxHealth { get; }
    public float DownedThreshold { get; }

    public event Action OnDeath;
    public event Action OnDowned;
    public event Action OnRecovered;
    public event Action<float, HealthStatus> OnHealthChanged;
    
    void TakeDamage(float damage);
    bool IsDowned();
    bool IsDead();
}

public enum HealthStatus
{
    Dead,
    Downed,
    Hurt,
    Healthy
}