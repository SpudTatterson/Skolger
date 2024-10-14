using UnityEditor;
using UnityEngine;

namespace Skolger.Tutorial
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class RaycastCellHelper
    {
        private static IRaycastCellHelperUser activeVisualAid;

        static Cell firstCell;
        static Cell lastCell;

        public static void StartEditModeRaycast(IRaycastCellHelperUser visualAid)
        {
#if UNITY_EDITOR
            activeVisualAid = visualAid;
            SceneView.duringSceneGui += OnSceneGUI;
#endif
        }
#if UNITY_EDITOR
        private static void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt && !e.control && !e.shift)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    firstCell = GridManager.Instance.GetCellFromPosition(hit.point);
                    // activeVisualAid.SetCells(hit.point); // get cell on mouse stop click and calculate the cells from there and feed into where ever needed
                    Debug.Log($"Raycast hit at: {hit.point}");

                    e.Use();
                }
            }
            if (e.type == EventType.MouseUp && e.button == 0 && !e.alt && !e.control && !e.shift)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    lastCell = GridManager.Instance.GetCellFromPosition(hit.point);
                    // activeVisualAid.SetCells(hit.point); // get cell on mouse stop click and calculate the cells from there and feed into where ever needed
                    Debug.Log($"Raycast hit at: {hit.point}");

                    e.Use();

                    SquarePlacementStrategy placementStrategy = new SquarePlacementStrategy();
                    placementStrategy.GetCells(firstCell, lastCell);

                    activeVisualAid.SetCells(placementStrategy.GetCells(firstCell, lastCell).ToArray());

                    SceneView.duringSceneGui -= OnSceneGUI;

                    EditorUtility.SetDirty(TutorialManager.Instance);

                    firstCell = null;
                    lastCell = null;
                }
            }


        }
#endif
    }
    public interface IRaycastCellHelperUser
    {
        void SetCells(Cell[] cells);
    }
}