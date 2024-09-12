using System;

[Serializable]
public class BaseMoodEffect
{
    public BaseMoodEffectSO effectData ;

    public float activeTime;

    public BaseMoodEffect(BaseMoodEffectSO baseEffectData)
    {
        this.effectData = baseEffectData;
        activeTime = 0;
    }
}