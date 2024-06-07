using System.Threading;
using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Enums;
using CrashKonijn.Goap.Interfaces;
using UnityEngine;

public class MeleeAttackAction : ActionBase<AttackData>, IInjectable
{
    private AttackConfigSO AttackConfig;

    public void Inject(DependencyInjector injector)
    {
        AttackConfig = injector.AttackConfig;
    }


    public override void Created()
    {

    }

    public override void Start(IMonoAgent agent, AttackData data)
    {
        data.Timer = AttackConfig.AttackDelay;
    }

    public override ActionRunState Perform(IMonoAgent agent, AttackData data, ActionContext context)
    {
        data.Timer -= context.DeltaTime;

        float distanceToTarget = Vector3.Distance(data.Target.Position, agent.transform.position);
        bool shouldAttack = data.Target != null && distanceToTarget <= AttackConfig.MeleeAttackRadius;

        if(shouldAttack)
        {
            agent.transform.LookAt(data.Target.Position);
        }

        return data.Timer > 0 ? ActionRunState.Continue : ActionRunState.Stop;
    }

    public override void End(IMonoAgent agent, AttackData data)
    {

    }
}