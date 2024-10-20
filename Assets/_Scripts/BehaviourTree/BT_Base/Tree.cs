using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        [ShowInInspector]protected Node root = null;
        internal bool rearrangeTree;

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

        protected void RearrangeTree()
        {
            root = SetupTree();
        }

        protected abstract Node SetupTree();
    }
}