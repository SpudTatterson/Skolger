using System.Collections.Generic;
using System.Runtime.InteropServices;
using BehaviorTree;
using Sirenix.OdinValidator.Editor.Validators;
using Unity.VisualScripting;
using UnityEngine;
using Tree = BehaviorTree.Tree;

public class RearrangeTree : Node
{
    Node lastTask;
    Tree root;

    public RearrangeTree(Node nodeToEvaluate, Tree rootToCheck)
    {
        lastTask = nodeToEvaluate;
        root = rootToCheck;
    }

    public override NodeState Evaluate()
    {
        if(lastTask.state == NodeState.SUCCESS)
        {
            root.rearrangeTree = true;
            state = NodeState.SUCCESS;
        }
        else if(lastTask.state == NodeState.RUNNING)
        {
            state = NodeState.RUNNING;
        }
        else if(lastTask.state == NodeState.FAILURE)
        {
            state = NodeState.FAILURE;
        }

        return state;
    }
}