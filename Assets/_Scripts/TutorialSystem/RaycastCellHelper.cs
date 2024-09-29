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
                    activeVisualAid.SetCellPosition(hit.point);
                    Debug.Log($"Raycast hit at: {hit.point}");

                    SceneView.duringSceneGui -= OnSceneGUI;

                    EditorUtility.SetDirty(TutorialManager.Instance);
                    e.Use();
                }
            }
        }
#endif
    }
    public interface IRaycastCellHelperUser
    {
        void SetCellPosition(Vector3 point);
    }
}