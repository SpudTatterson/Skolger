using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;

public class GridBuilderTool : EditorWindow
{

    //General Vars
    GridManager gridManager;
    LayerManager layerManager;
    public static GridBuilderTool instance;

    GridSculptingStrategy sculptingStrategy;
    GridPaintingStrategy paintingStrategy;
    BuildingPlacerStrategy placerStrategy;
    FoliageSpreaderStrategy spreaderStrategy;

    string[] tools = { "World Sculpting", "World Painting", "Building Placer", "Foliage Spreader" };
    ActiveTool activeTool;
    enum ActiveTool
    {
        WorldSculpting,
        WorldPainting,
        BuildingPlacer,
        FoliageSpreader
    }

    [MenuItem("Tools/Grid/Grid Builder Tool")]
    public static void ShowWindow()
    {
        instance = GetWindow<GridBuilderTool>("Grid Builder Tool");
    }
    void OnEnable()
    {
        instance = this;
        gridManager = FindAnyObjectByType<GridManager>();
        layerManager = FindAnyObjectByType<LayerManager>();

        sculptingStrategy = new GridSculptingStrategy(gridManager, layerManager);
        BrushToolManager.RegisterTool(sculptingStrategy);
        placerStrategy = new BuildingPlacerStrategy(gridManager, layerManager);
        spreaderStrategy = new FoliageSpreaderStrategy(gridManager, layerManager);
        BrushToolManager.RegisterTool(spreaderStrategy);
        paintingStrategy = new GridPaintingStrategy(gridManager, layerManager);
        BrushToolManager.RegisterTool(paintingStrategy);
    }
    void OnDisable()
    {
        BrushToolManager.UnregisterTool(spreaderStrategy);
        BrushToolManager.UnregisterTool(paintingStrategy);
        BrushToolManager.UnregisterTool(sculptingStrategy);
    }


    void OnGUI()
    {
        GUILayout.Label("Grid Builder Tool", EditorStyles.boldLabel);

        activeTool = (ActiveTool)GUILayout.Toolbar((int)activeTool, tools);

        if (activeTool == ActiveTool.WorldSculpting)
            sculptingStrategy.OnGUI();
        else if (activeTool == ActiveTool.BuildingPlacer)
            placerStrategy.OnGUI();
        else if (activeTool == ActiveTool.FoliageSpreader)
            spreaderStrategy.OnGUI();
        else if (activeTool == ActiveTool.WorldPainting)
            paintingStrategy.OnGUI();
    }



    void OnSceneGUI(SceneView sceneView)
    {
        if (gridManager == null) return;

        if (activeTool == ActiveTool.WorldSculpting)
            sculptingStrategy.OnSceneGUI();
        else if (activeTool == ActiveTool.BuildingPlacer)
            placerStrategy.OnSceneGUI();
        else if (activeTool == ActiveTool.FoliageSpreader)
            spreaderStrategy.OnSceneGUI();
        else if (activeTool == ActiveTool.WorldPainting)
            paintingStrategy.OnSceneGUI();
    }


    #region WorldSculpting

    [Shortcut("GridTools/StartGridBuilding", KeyCode.F, ShortcutModifiers.Alt)]
    static void StartPaintingShortcut()
    {
        GridBuilderTool window = GetWindow<GridBuilderTool>();
        window.StartSculpting();
    }

    void StartSculpting()
    {
        BrushToolManager.DisableAllBrushTools();
        activeTool = ActiveTool.WorldSculpting;
        sculptingStrategy.StartTool();
    }

    #endregion


    void OnBecameVisible()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnBecameInvisible()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    void OnDestroy()
    {
        instance = null;
    }
}