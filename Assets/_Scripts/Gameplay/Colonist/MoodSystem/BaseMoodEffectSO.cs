using UnityEngine;

[CreateAssetMenu(menuName = "Colonist/BaseMoodEffect")]
public class BaseMoodEffectSO : ScriptableObject
{
    public string effectName;
    public float effectTime;
    public int effect;
    public bool stackable;
}
