using UnityEngine;

[CreateAssetMenu(menuName = "AI/Attack Config", fileName = "AtackConfig", order = 1)]
public class AttackConfigSO : ScriptableObject
{
    public float SensorRaduis = 10f;
    public float MeleeAttackRadius = 1f;
    public int MeleeAttackCost = 1;
    public float AttackDelay = 1f;
    public LayerMask AttackableLayerMask;
}