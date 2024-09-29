namespace Skolger.Tutorial
{
    [System.Serializable]
    public abstract class VisualAid
    {
        public BaseStep parentStep {get; private set;} 
        public bool finished { get; protected set; }
        
        public void SetParent(BaseStep parent)
        {
            parentStep = parent;
        }
        public abstract void Initialize();
        public abstract void Update();
        public abstract void Reset();
    }
}
