using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class AutoRefreshUpdate
{
    private static double lastCheckTime;
    private const double checkInterval = 1.0; // Interval in seconds
    private static string[] lastFileStates;

    static AutoRefreshUpdate()
    {
        EditorApplication.update += Update;
        lastFileStates = GetFileStates();
    }

    private static void Update()
    {
        if (EditorApplication.timeSinceStartup - lastCheckTime > checkInterval)
        {
            lastCheckTime = EditorApplication.timeSinceStartup;
            CheckForChanges();
        }
    }

    private static void CheckForChanges()
    {
        string[] currentFileStates = GetFileStates();

        if (!AreFileStatesEqual(lastFileStates, currentFileStates))
        {
            lastFileStates = currentFileStates;
            AssetDatabase.Refresh();
            Debug.Log("Project refreshed due to script change.");
        }
    }

    private static string[] GetFileStates()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        string[] fileStates = new string[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            fileStates[i] = files[i] + File.GetLastWriteTime(files[i]).ToString();
        }

        return fileStates;
    }

    private static bool AreFileStatesEqual(string[] oldStates, string[] newStates)
    {
        if (oldStates.Length != newStates.Length)
            return false;

        for (int i = 0; i < oldStates.Length; i++)
        {
            if (oldStates[i] != newStates[i])
                return false;
        }

        return true;
    }
}