using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class BuildingPrefabCreator : EditorWindow
{
    string prefabName;
    GameObject baseBuildingPrefab;
    GameObject visual;
    Material material;
    string folderPath;
    bool disableObstacle;

    Vector2 scrollPosition;

    [MenuItem("Tools/Prefabs/BuildingPrefabCreator")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(BuildingPrefabCreator));
        window.titleContent = new GUIContent("Create New Prefab");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create New Prefab", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10f);

        GUILayout.Label("Prefab Settings", EditorStyles.boldLabel);
        baseBuildingPrefab = EditorGUILayout.ObjectField("Base Building Prefab", baseBuildingPrefab, typeof(GameObject), true) as GameObject;
        visual = EditorGUILayout.ObjectField("Model", visual, typeof(GameObject), true) as GameObject;
        material = EditorGUILayout.ObjectField("Material", material, typeof(Material), true) as Material;
        prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);
        disableObstacle = EditorGUILayout.Toggle("Disable Obstacle", disableObstacle);

        GUILayout.Space(10f);

        if (GUILayout.Button("Select Folder", GUILayout.Height(30f)))
        {
            folderPath = EditorUtility.OpenFolderPanel("Choose Folder to Save Prefabs", "Assets/_Prefabs/Placeables", "");
        }

        GUILayout.Label($"Selected Folder: {folderPath}");

        GUILayout.Space(20f);

        if (GUILayout.Button("Create Building", GUILayout.Height(30f)))
        {
            CreateBuilding();
        }

        EditorGUILayout.EndScrollView();
    }

    void CreateBuilding()
    {
        if (baseBuildingPrefab == null || visual == null || string.IsNullOrEmpty(prefabName) || string.IsNullOrEmpty(folderPath))
        {
            Debug.LogWarning("Please ensure all fields are filled out and a folder is selected before creating a building.");
            return;
        }

        // Instantiate the base building prefab
        GameObject buildingPrefab = (GameObject)PrefabUtility.InstantiatePrefab(baseBuildingPrefab);
        buildingPrefab.name = prefabName;
        buildingPrefab.layer = LayerManager.Instance.buildableLayer;

        // Set up the visual model
        Transform visualParent = buildingPrefab.transform.GetChild(0);
        GameObject model = Instantiate(visual, visualParent);
        model.transform.localPosition = Vector3.zero;

        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();

        // Get the calculated bounds of the visual
        Box box = Box.GetBoxSize(model);
        box = Box.RoundBoxToHalf(box);

        // Adjust the NavMeshObstacle
        NavMeshObstacle navMeshObstacle = buildingPrefab.GetComponent<NavMeshObstacle>();
        if (disableObstacle)
        {
            navMeshObstacle.enabled = false;
        }
        else
        {
            navMeshObstacle.size = box.halfExtents;
            navMeshObstacle.center = box.center;
        }

        // Adjust the BoxCollider
        BoxCollider collider = buildingPrefab.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.size = box.halfExtents;
            collider.center = box.center;
        }
        else
        {
            Debug.LogWarning("No BoxCollider found on the base building prefab.");
        }

        if (material != null)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.sharedMaterial = material;
            }
        }

        // Save the prefab as a variant
        string fullPath = $"{folderPath}/{prefabName}.prefab";
        PrefabUtility.SaveAsPrefabAssetAndConnect(buildingPrefab, fullPath, InteractionMode.UserAction);

        Debug.Log($"Building prefab variant '{prefabName}' created and saved at {fullPath}.");

        // Create and save the construction site variant
        buildingPrefab.name = $"{prefabName}_ConstructionSite";
        fullPath = $"{folderPath}/{prefabName}_ConstructionSite.prefab";

        navMeshObstacle.enabled = false;

        foreach (Renderer renderer in renderers)
        {
            renderer.sharedMaterial = MaterialManager.Instance.materials.unfinishedBuildingMaterial;
        }

        DestroyImmediate(buildingPrefab.GetComponent<BuildingObject>());
        buildingPrefab.AddComponent<ConstructionSiteObject>();

        PrefabUtility.SaveAsPrefabAssetAndConnect(buildingPrefab, fullPath, InteractionMode.UserAction);

        Debug.Log($"Construction site prefab '{prefabName}_ConstructionSite' created and saved at {fullPath}.");

        DestroyImmediate(buildingPrefab);
    }
}
