using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Interfaces;



public class DependencyInjector : GoapConfigInitializerBase, IGoapInjector
{
    public AttackConfigSO AttackConfig;

    public override void InitConfig(GoapConfig config)
    {
        config.GoapInjector = this;
    }

    public void Inject(IActionBase action)
    {
        if (action is IInjectable injectable)
        {
            injectable.Inject(this);
        }
    }

    public void Inject(IGoalBase goal)
    {

    }

    public void Inject(IWorldSensor worldSensor)
    {

    }

    public void Inject(ITargetSensor targetSensor)
    {

    }
}
