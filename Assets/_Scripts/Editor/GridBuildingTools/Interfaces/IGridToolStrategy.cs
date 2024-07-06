using UnityEngine;

public interface IGridToolStrategy 
{
    void OnGUI();
    void OnSceneGUI();

    void StartTool();
}