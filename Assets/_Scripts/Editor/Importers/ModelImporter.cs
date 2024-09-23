using UnityEditor;
using UnityEngine;

public class ModelImporter : AssetPostprocessor
{
    // This method is called for each model during the import process
    void OnPreprocessModel()
    {
        // Use Unity's built-in ModelImporter (not the custom class)
        UnityEditor.ModelImporter modelImporter = assetImporter as UnityEditor.ModelImporter;

        if (modelImporter != null)
        {
            // Ensure Read/Write Enabled is set to true
            if (!modelImporter.isReadable)
            {
                modelImporter.isReadable = true;
            }
        }
    }
}
