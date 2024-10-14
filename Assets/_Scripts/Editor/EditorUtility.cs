using System;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace SpudsUtility
{
    public class UtilityEditor
    {
        public static void DrawDragAndDropArea<T>(Action<UnityEngine.Object> foreachAction, string dropAreaText = "", string objectName = "") where T : UnityEngine.Object
        {
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, dropAreaText);

            // Display ObjectField for selection
            T givenObject = EditorGUILayout.ObjectField($"Select {objectName}", null, typeof(T), true) as T;

            // If an object is selected via ObjectField, perform the action
            if (givenObject != null)
            {
                foreachAction(givenObject);
            }

            // Handle Drag and Drop events
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is T)
                            {
                                foreachAction(draggedObject);
                            }
                        }
                    }
                    Event.current.Use();
                    break;
            }
        }
    }
}
