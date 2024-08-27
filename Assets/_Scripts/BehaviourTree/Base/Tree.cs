using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        [ShowInInspector]protected Node root = null;

        protected void Start()
        {
            root = SetupTree();
        }

        protected virtual void Update()
        {
            if (root != null)
            {
                root.Evaluate();           
            }
        }

        protected abstract Node SetupTree();
    }
}