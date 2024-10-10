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
    FoliageSpreaderStrategy foliageStrategy;
    ItemSpreaderStrategy itemStrategy;

    string[] tools = { "World Sculpting", "World Painting", "Building Placer", "Foliage Spreader", "Item Spreader" };
    ActiveTool activeTool;
    enum ActiveTool
    {
        WorldSculpting,
        WorldPainting,
        BuildingPlacer,
        FoliageSpreader,
        ItemSpreader,
    }

    [MenuItem("Tools/Grid/Grid Builder Tool")]
    public static void ShowWindow()
    {
        instance = GetWindow<GridBuilderTool>("Grid Builder Tool");
    }
    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        instance = this;
        gridManager = FindAnyObjectByType<GridManager>();
        layerManager = FindAnyObjectByType<LayerManager>();

        sculptingStrategy = new GridSculptingStrategy(gridManager, layerManager);
        BrushToolManager.RegisterTool(sculptingStrategy);
        placerStrategy = new BuildingPlacerStrategy(gridManager, layerManager);
        foliageStrategy = new FoliageSpreaderStrategy(gridManager, layerManager);
        BrushToolManager.RegisterTool(foliageStrategy);
        paintingStrategy = new GridPaintingStrategy(gridManager, layerManager);
        BrushToolManager.RegisterTool(paintingStrategy);
        itemStrategy = new ItemSpreaderStrategy(gridManager, layerManager);
        BrushToolManager.RegisterTool(itemStrategy);
    }
    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        BrushToolManager.UnregisterTool(foliageStrategy);
        BrushToolManager.UnregisterTool(paintingStrategy);
        BrushToolManager.UnregisterTool(sculptingStrategy);
        BrushToolManager.UnregisterTool(itemStrategy);
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
            foliageStrategy.OnGUI();
        else if (activeTool == ActiveTool.WorldPainting)
            paintingStrategy.OnGUI();
        else if (activeTool == ActiveTool.ItemSpreader)
            itemStrategy.OnGUI();
    }



    void OnSceneGUI(SceneView sceneView)
    {
        if (gridManager == null) return;

        if (activeTool == ActiveTool.WorldSculpting)
            sculptingStrategy.OnSceneGUI();
        else if (activeTool == ActiveTool.BuildingPlacer)
            placerStrategy.OnSceneGUI();
        else if (activeTool == ActiveTool.FoliageSpreader)
            foliageStrategy.OnSceneGUI();
        else if (activeTool == ActiveTool.WorldPainting)
            paintingStrategy.OnSceneGUI();
        else if (activeTool == ActiveTool.ItemSpreader)
            itemStrategy.OnSceneGUI();
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

    void OnDestroy()
    {
        instance = null;
    }
}