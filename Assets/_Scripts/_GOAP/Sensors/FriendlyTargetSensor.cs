using System.Runtime.CompilerServices;
using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Interfaces;
using CrashKonijn.Goap.Sensors;
using UnityEngine;

public class FriendlyTargetSensor : LocalTargetSensorBase, IInjectable
{
    private AttackConfigSO AttackConfig;
    private Collider[] Colliders = new Collider[1];

    public override void Created()
    {

    }

    public override void Update()
    {

    }

    public override ITarget Sense(IMonoAgent agent, IComponentReference references)
    {
        if (Physics.OverlapSphereNonAlloc(agent.transform.position, AttackConfig.SensorRaduis, Colliders, AttackConfig.AttackableLayerMask) > 0)
        {
            return new TransformTarget(Colliders[0].transform);
        }

        return null;
    }

    public void Inject(DependencyInjector injector)
    {
        AttackConfig = injector.AttackConfig;
    }
}