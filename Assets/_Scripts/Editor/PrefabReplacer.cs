using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class PrefabReplacer : EditorWindow
{

    UnityEngine.Object prefabToReplace;
    bool resetScale;


    [MenuItem("Tools/Prefabs/PrefabReplacer")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(PrefabReplacer));
    }
    private void OnGUI()
    {
        GUILayout.Label("Replace selection with given prefab", EditorStyles.boldLabel);

        prefabToReplace = EditorGUILayout.ObjectField("Model", prefabToReplace, typeof(UnityEngine.Object), true) as UnityEngine.Object;
        resetScale = EditorGUILayout.Toggle("Reset Scale", resetScale);

        if (GUILayout.Button("Replace Prefabs"))
        {
            ReplacePrefab();
        }
    }

    void ReplacePrefab()
    {
        if (prefabToReplace == null)
        {
            Debug.LogWarning("Prefab is not assigned.");
            return;
        }

        Undo.RegisterCompleteObjectUndo(Selection.gameObjects, "Replace with Prefab");

        foreach (var selectedObject in Selection.gameObjects)
        {
            var newObject = PrefabUtility.InstantiatePrefab(prefabToReplace) as GameObject;
            newObject.transform.position = selectedObject.transform.position;
            newObject.transform.rotation = selectedObject.transform.rotation;

            if(!resetScale)
            {
               newObject.transform.localScale = selectedObject.transform.localScale; 
            }
            
            newObject.transform.parent = selectedObject.transform.parent;

            Undo.RegisterCreatedObjectUndo(newObject, "Replace with Prefab");
            Undo.DestroyObjectImmediate(selectedObject);
        }
    }
}
