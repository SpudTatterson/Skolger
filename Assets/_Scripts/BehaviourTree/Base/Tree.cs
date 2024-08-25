using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        [ShowInInspector]private Node root = null;

        protected void Start()
        {
            root = SetupTree();
        }

        private void Update()
        {
            if (root != null)
            {
                root.Evaluate();           
            }
        }

        protected abstract Node SetupTree();
    }
}