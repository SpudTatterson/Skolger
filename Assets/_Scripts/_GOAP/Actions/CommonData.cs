using CrashKonijn.Goap.Interfaces;
using UnityEngine;

public class CommonData : IActionData
{
    public ITarget Target { get; set; }
    public float Timer { get; set; }
}