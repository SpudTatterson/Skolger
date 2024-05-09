using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class PrefabCreator : EditorWindow
{
    string PrefabName;
    string ContainerName = "GFX";
    GameObject GivenModel;
    string prefabPath = "Assets/_Prefabs/";
    bool MakeBoxCollider;
    bool MakeCapsuleCollider;
    bool MakeCylinderCollider;
    bool SetPivotToBottom;
    bool SetPivotToTop;
    bool makeDestroyable;
    Vector2 scrollPosition;
    

    

    //Rect rect = new Rect(60f,5f,150f,30f);

    [MenuItem("Tools/Prefabs/PrefabCreator")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(PrefabCreator));
        //window.titleContent.text = "Create New Prefab";

    }
     private void OnGUI()
    {
        
        GUILayout.Label("Create New Prefab", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10f);
        
        if (GUILayout.Button("Create Prefab", GUILayout.Height(30f)))
        {
            CreatePrefab();
        }
        GUILayout.Space(10f);

        GUILayout.Label("Prefab Settings", EditorStyles.boldLabel);
        GivenModel = EditorGUILayout.ObjectField("Model", GivenModel, typeof(GameObject), true) as GameObject;
        PrefabName = EditorGUILayout.TextField("Prefab Name", PrefabName);
        ContainerName = EditorGUILayout.TextField(new GUIContent("Container Name", "the name of the parent that will hold all graphical components of the prefab"), ContainerName);
        prefabPath = EditorGUILayout.TextField(new GUIContent("Prefab Path", "Where the prefab will be stored"), prefabPath);

        GUILayout.Space(10f);

        GUILayout.Label("Collider Options", EditorStyles.boldLabel);
        MakeBoxCollider = EditorGUILayout.Toggle("Generate Box Collider", MakeBoxCollider);
        MakeCapsuleCollider = EditorGUILayout.Toggle("Generate Capsule Collider", MakeCapsuleCollider);
        MakeCylinderCollider = EditorGUILayout.Toggle(new GUIContent("Generate \"Cylinder\" Collider","puts a box collider at the top and bottom of the mesh and a capsule collider between them"), MakeCylinderCollider);

        GUILayout.Space(10f);

        GUILayout.Label("Pivot Options", EditorStyles.boldLabel);
        SetPivotToBottom = EditorGUILayout.Toggle("Set Pivot To Bottom", SetPivotToBottom);
        SetPivotToTop = EditorGUILayout.Toggle("Set Pivot To Top", SetPivotToTop);

        GUILayout.Space(10f);

        GUILayout.Label("Destroyable Prefab", EditorStyles.boldLabel);
        makeDestroyable = EditorGUILayout.Toggle(new GUIContent("Destroyed Prefab",
        "Will give a rigidbody and convex mesh collider to each object with a mesh renderer and give the parent the explode script"),
        makeDestroyable);

        GUILayout.Space(20f);

        if (GUILayout.Button("Create Prefab", GUILayout.Height(30f)))
        {
            CreatePrefab();
        }

        EditorGUILayout.EndScrollView();
    }

    
    private void CreatePrefab()
    {
        //Check for issues
        if(GivenModel == null)
        {
            Debug.LogError("No Model Given");
            return;
        }
        if(PrefabName == null || PrefabName == "")
        {
            PrefabName = GivenModel.name;
        }
        if(ContainerName == null)
        {
            ContainerName = "GFX";
        }
        string directoryPath = Path.GetDirectoryName(prefabPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log("Path created: " + directoryPath);
        }
        
        //Make all necessary vars
        string fixedPrefabName = PrefabName.Replace("(Clone)", "");
        GameObject parent = new GameObject(fixedPrefabName);
        GameObject parentGFX = new GameObject(ContainerName);
        GameObject ColliderParent = new GameObject("Colliders");
        Vector3 Position = new Vector3();
        Quaternion rotation = GivenModel.transform.rotation;   
        GameObject Model = Instantiate(GivenModel, Position, rotation);

        //Set Parent position
        if(SetPivotToBottom)
            parent.transform.position = GetBottom(Model);
        if(SetPivotToTop)
            parent.transform.position = GetTop(Model);

        //set hierarchy
        Model.transform.SetParent(parentGFX.transform);
        parentGFX.transform.SetParent(parent.transform);
        ColliderParent.transform.SetParent(parent.transform);
        
        //fix name
        GameObject OldModel = Instantiate(Model, Position, rotation);
        Model.name = Model.name.Replace("(Clone)", "");

        //generate colliders
        if(makeDestroyable)
        {
            MakeDestroyable(Model, parent);
        }
        if(MakeBoxCollider)
            GenerateBoxCollider(GivenModel, ColliderParent);
        if (MakeCapsuleCollider)
            GenerateCapsuleCollider(GivenModel, ColliderParent); 
        if(MakeCylinderCollider)
            GenerateCylinderColliders(GivenModel, ColliderParent);
    
        //CleanUp
        DestroyImmediate(OldModel);
        
        //make prefab path
        string newPrefabPath = prefabPath + fixedPrefabName + ".prefab";

        //check if prefab already exists
        bool prefabExists = File.Exists(newPrefabPath);
        if (prefabExists)
        {
            bool confirmOverride = EditorUtility.DisplayDialog("Prefab Override",
                "The prefab already exists. Are you sure you want to override it?",
                "Yes", "No");
            if (!confirmOverride)
            {
                DestroyImmediate(parent);
                return;
            }
        }
            
        //save prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(parent, newPrefabPath);
        DestroyImmediate(parent);

        Debug.Log("Prefab created: " + AssetDatabase.GetAssetPath(prefab));
        PrefabName = "";
    }

    private void GenerateBoxCollider(GameObject Model, GameObject ColliderParent)
    {
        Bounds bounds = GetBounds(Model);
        GameObject newChild = new GameObject("Box Collider");
        newChild.transform.SetParent(ColliderParent.transform);

        BoxCollider collider = newChild.gameObject.AddComponent<BoxCollider>();
        collider.center = bounds.center;
        collider.size = bounds.size;
    }
    private void GenerateCapsuleCollider(GameObject Model, GameObject ColliderParent)
    {
        Bounds bounds = GetBounds(Model);
        GameObject newChild = new GameObject("Capsule Collider");
        newChild.transform.SetParent(ColliderParent.transform);

        CapsuleCollider collider = newChild.gameObject.AddComponent<CapsuleCollider>();
        collider.center = bounds.center;
        collider.radius = Mathf.Max(bounds.extents.x, bounds.extents.z);
        collider.height = bounds.size.y;
        collider.direction = 1; // Set the direction to Y-axis

        // Adjust the collider position to match the model's position
        collider.transform.position = ColliderParent.transform.position;        
    }


    private void GenerateCylinderColliders(GameObject model, GameObject colliderParent)
    {       
        Bounds bounds = GetBounds(model);
        float height = bounds.size.y;
        float radius = Mathf.Max(bounds.size.x, bounds.size.z) * 0.5f;
        GameObject capsuleChild = new GameObject("Capsule Collider");
        capsuleChild.transform.SetParent(colliderParent.transform);
        GameObject BottomBoxChild = new GameObject("Bottom Collider");
        BottomBoxChild.transform.SetParent(colliderParent.transform);
        GameObject TopBoxChild = new GameObject("Top Collider");
        TopBoxChild.transform.SetParent(colliderParent.transform);


        // Add a CapsuleCollider
        CapsuleCollider capsuleCollider = capsuleChild.AddComponent<CapsuleCollider>();
        capsuleCollider.direction = 1; // Set the direction to represent the height of the object
        capsuleCollider.center = bounds.center; // Center the collider
        capsuleCollider.radius = radius; // Set the radius based on the dimensions of the model
        capsuleCollider.height = height; // Set the height based on the dimensions of the model

        // Calculate the top and bottom positions of the collider
        Vector3 topPosition = capsuleCollider.center + new Vector3(0f, height * 0.5f, 0f);
        Vector3 bottomPosition = capsuleCollider.center - new Vector3(0f, height * 0.5f, 0f);

        // Calculate the adjusted size for the box colliders
        float adjustedWidth = radius * 1.4f;

        // Add a BoxCollider at the top
        BoxCollider topCollider = TopBoxChild.AddComponent<BoxCollider>();
        topCollider.center = colliderParent.transform.InverseTransformPoint(topPosition);
        topCollider.size = new Vector3(adjustedWidth, 0.01f, adjustedWidth); // Adjust the size of the top collider

        // Add a BoxCollider at the bottom
        BoxCollider bottomCollider = BottomBoxChild.AddComponent<BoxCollider>();
        bottomCollider.center = colliderParent.transform.InverseTransformPoint(bottomPosition);
        bottomCollider.size = new Vector3(adjustedWidth, 0.01f, adjustedWidth); // Adjust the size of the bottom collider
    }


    private Vector3 GetTop(GameObject model)
    {
        
        Bounds bounds = GetBounds(model);
        Vector3 top = bounds.center + new Vector3(0f, bounds.extents.y, 0f);
        return top;
    }
    private Vector3 GetBottom(GameObject model)
    {
        
        Bounds bounds = GetBounds(model);
        Vector3 bottom = bounds.center - new Vector3(0f, bounds.extents.y, 0f);
        return bottom;
    }

    void MakeDestroyable(GameObject model, GameObject parent)
    {
        MeshFilter[] meshFilters = model.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            meshFilters[i].gameObject.AddComponent<Rigidbody>();
            MeshCollider collider = meshFilters[i].gameObject.AddComponent<MeshCollider>();
            collider.convex = true;

        }
        //un comment this when you make the explode script
        //parent.AddComponent<Explode>();
    }

    private Type GetTypeFromString(string ScriptName)
    {
        
        return System.Type.GetType(ScriptName, true);
    }

    private Bounds GetBounds(GameObject model)
    {
        Vector3 oldTransform = model.transform.position;
        model.transform.position = Vector3.zero;        
        Renderer renderer = model.GetComponent<Renderer>();
        Bounds bounds = new Bounds();
        if(renderer != null)
            bounds.Encapsulate(renderer.bounds);
        else
        {
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Debug.LogError("No renderers found in the model.");
                return bounds;
             }
            foreach (Renderer r in renderers)
            {
                bounds.Encapsulate(r.bounds);
            }  
                     
        }
        model.transform.position = oldTransform;
        return bounds;
    }
}
