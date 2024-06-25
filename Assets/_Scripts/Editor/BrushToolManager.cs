using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public static class BrushToolManager
{
    private static List<IBrushTool> brushTools = new List<IBrushTool>();

    public static void RegisterTool(IBrushTool tool)
    {
        if (!brushTools.Contains(tool))
        {
            brushTools.Add(tool);
        }
    }

    public static void UnregisterTool(IBrushTool tool)
    {
        if (brushTools.Contains(tool))
        {
            brushTools.Remove(tool);
        }
    }

    [Shortcut("BrushTools/IncreaseBrushSize", KeyCode.RightBracket)]
    static void IncreaseBrushSizeShortcut()
    {
        foreach (var tool in brushTools)
        {
            if (tool.isPainting)
            {
                tool.IncreaseBrushSize();

            }
        }
    }

    [Shortcut("BrushTools/DecreaseBrushSize", KeyCode.LeftBracket)]
    static void DecreaseBrushSizeShortcut()
    {
        foreach (var tool in brushTools)
        {
            if (tool.isPainting)
            {
                tool.DecreaseBrushSize();

            }
        }
    }
    public static void DisableAllBrushTools()
    {
        foreach (var tool in brushTools)
        {
            if(tool == null)
            {
                continue;
            }

            tool.StopPainting();
        }
    }
}