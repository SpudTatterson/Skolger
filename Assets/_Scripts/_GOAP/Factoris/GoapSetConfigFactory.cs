using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Classes.Builders;
using CrashKonijn.Goap.Configs.Interfaces;
using CrashKonijn.Goap.Enums;
using Comparison = CrashKonijn.Goap.Resolver.Comparison;
using UnityEngine;

[RequireComponent(typeof(DependencyInjector))]
public class GoapSetConfigFactory : GoapSetFactoryBase
{
    private DependencyInjector Injector;

    public override IGoapSetConfig Create()
    {
        Injector = GetComponent<DependencyInjector>();
        GoapSetBuilder builder = new("AxeManSet");

        BuildGoals(builder);
        BuildActions(builder);
        BuildSensors(builder);

        return builder.Build();
    }

    private void BuildGoals(GoapSetBuilder builder)
    {
        builder.AddGoal<WanderGoal>()
            .AddCondition<IsWandering>(Comparison.GreaterThanOrEqual, 1);

        builder.AddGoal<KillFriendly>()
            .AddCondition<FriendlyHealth>(Comparison.SmallerThanOrEqual, 0);
    }

    private void BuildActions(GoapSetBuilder builder)
    {
        builder.AddAction<WanderAction>()
            .SetTarget<WanderTarget>()
            .AddEffect<IsWandering>(EffectType.Increase)
            .SetBaseCost(5)
            .SetInRange(1);

        builder.AddAction<MeleeAttackAction>()
            .SetTarget<FriendlyTarget>()
            .AddEffect<FriendlyHealth>(EffectType.Decrease)
            .SetBaseCost(Injector.AttackConfig.MeleeAttackCost)
            .SetInRange(Injector.AttackConfig.SensorRaduis);

    }

    private void BuildSensors(GoapSetBuilder builder)
    {
        builder.AddTargetSensor<WanderTargetSensor>()
            .SetTarget<WanderTarget>();

        builder.AddTargetSensor<FriendlyTargetSensor>()
            .SetTarget<FriendlyTarget>();
    }
}