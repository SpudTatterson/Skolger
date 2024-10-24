using System;

namespace Skolger.Tutorial
{
    public interface INumberedStep
    {
        float max { get; }
        float current { get; }

        event Action<float> OnNumberChange;
    }
}

