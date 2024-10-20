using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;

namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }
    [System.Serializable]
    public abstract class Node
    {
        [ShowInInspector] protected internal NodeState state;
        public Node parent;
        [ShowInInspector] protected internal List<Node> children = new List<Node>();
        public int priority { get; set; }
        public bool flaggedRoot { get; set; }

        [ShowInInspector] private Dictionary<Enum, object> dataContext = new Dictionary<Enum, object>();

        public Node()
        {
            parent = null;
            priority = 0;
            flaggedRoot = false;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                Attach(child);
            }
        }

        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public abstract NodeState Evaluate();

        public void SetData(Enum key, object value)
        {
            dataContext[key] = value;
        }

        public Node GetRootNode()
        {
            Node root = this;
            while (root.parent != null)
            {
                root = root.parent;
            }
            return root;
        }

        public Node GetFlaggedRootNode()
        {
            Node root = this;
            while (root.parent != null && !flaggedRoot)
            {
                root = root.parent;
            }
            return root;
        }

        public void SetDataOnRoot(Enum key, object value)
        {
            Node root = GetRootNode();
            root.SetData(key, value);
        }

        public void SetDataOnFlaggedRoot(Enum key, object value)
        {
            Node root = GetFlaggedRootNode();
            root.SetData(key, value);
        }

        public object GetData(Enum key)
        {
            object value = null;
            if (dataContext.TryGetValue(key, out value))
            {
                return value;
            }

            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                {
                    return value;
                }
                node = node.parent;
            }
            return null;
        }

        public bool ClearData(Enum key)
        {
            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                {
                    return true;
                }
                node = node.parent;
            }
            return false;
        }
    }
}
