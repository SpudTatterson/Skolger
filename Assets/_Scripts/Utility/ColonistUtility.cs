using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class ColonistUtility
{
    public static void AddColonistToBoard(string name, ColonistData colonist)
    {
        var colonistsDataBar = MonoBehaviour.Instantiate(UIManager.Instance.colonistDataPrefab, UIManager.Instance.colonistsInfoBoard.transform);
        var data = colonistsDataBar.GetComponent<ColonistBar>();
        data.SetDataOnCreation(name, colonist);
    }
    static readonly List<string> firstNames = new List<string>
        {
            "Erik",
            "Bjorn",
            "Sigrid",
            "Leif",
            "Astrid",
            "Olaf",
            "Freya",
            "Ivar",
            "Gunnar",
            "Helga",
            "Ragnhild",
            "Sven",
            "Ingrid",
            "Harald",
            "Ragnar"
        };

    static readonly List<string> lastNames = new List<string>
        {
            "Halden",
            "Strand",
            "Berg",
            "Fjord",
            "Alfheim",
            "Hamar",
            "Kjell",
            "Vik",
            "Skog",
            "Lothbrok",
            "Dal",
            "Stav",
            "Voll",
            "Ask",
            "Grove",
        };

    public static Sprite CaptureFace(GameObject objectToCapture, float faceHeight, Vector3 offset, int width, int height, float renderDistance)
    {
        RenderTexture renderTexture = new RenderTexture(width, height, 32);

        GameObject facePosition = Object.Instantiate(new GameObject(), objectToCapture.transform);
        GameObject cameraObject = Object.Instantiate(new GameObject(), objectToCapture.transform);
        Camera captureCamera = cameraObject.AddComponent<Camera>();

        captureCamera.targetTexture = renderTexture;
        captureCamera.orthographic = false;
        captureCamera.fieldOfView = 60;
        captureCamera.clearFlags = CameraClearFlags.SolidColor;
        captureCamera.backgroundColor = Color.clear;
        captureCamera.farClipPlane = renderDistance;

        facePosition.transform.position += new Vector3(0, faceHeight, 0);
        captureCamera.transform.position += objectToCapture.transform.TransformDirection(offset);
        captureCamera.transform.LookAt(facePosition.transform.position);

        RenderTexture.active = renderTexture;
        captureCamera.Render();

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        RenderTexture.active = null;
        Object.Destroy(cameraObject);
        Object.Destroy(renderTexture);
        Object.Destroy(facePosition);

        return sprite;
    }

    public static string SetRandomName()
    {
        int firstName = Random.Range(0, firstNames.Count);
        int lastName = Random.Range(0, lastNames.Count);

        return firstNames[firstName] + " " + lastNames[lastName];
    }


    public static bool ReachedDestinationOrGaveUp(NavMeshAgent agent)
    {
        if (agent.pathPending) return false;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }

        return false;
    }
}
