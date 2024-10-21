using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class ColonistUtility
{
    public static void AddColonistToBoard(string name, ColonistData colonist)
    {
        UIManager.Instance.colonistBoard.CreateNewColonist(colonist);
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

        GameObject empty = new GameObject();
        GameObject facePosition = Object.Instantiate(empty, objectToCapture.transform);
        GameObject cameraObject = Object.Instantiate(empty, objectToCapture.transform);
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
        Object.Destroy(empty);

        return sprite;
    }

    public static string SetRandomName()
    {
        int firstName = Random.Range(0, firstNames.Count);
        int lastName = Random.Range(0, lastNames.Count);

        return $"{firstNames[firstName]} {lastNames[lastName]}";
    }

    public static bool ReachedDestination(this NavMeshAgent agent, Vector3 target)
    {
        float distanceFromTarget = Vector3.Distance(agent.transform.position, target);

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance + 0.1f && distanceFromTarget <= agent.stoppingDistance + 0.1f)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static Vector3 ConvertToVector3(object objectToVector3)
    {
        if (objectToVector3 is Vector3 vector3)
        {
            return vector3;
        }
        else if (objectToVector3 is MonoBehaviour monoBehaviour)
        {
            objectToVector3 = monoBehaviour.transform.position;
        }
        else if (objectToVector3 is Cell cell)
        {
            objectToVector3 = cell.position;
        }
        else
        {
            Debug.LogError("Was not able to convert to vector3");
            return Vector3.zero;
        }

        return (Vector3)objectToVector3;
    }

    public static bool CanReachPoint(this NavMeshAgent agent, Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(targetPosition, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                return true; // The path is valid and reachable
            }
        }
        else
        {
            Vector3 adjustedPos = targetPosition +
             VectorUtility.GetDirection(targetPosition, agent.transform.position) * agent.stoppingDistance;
            Debug.DrawLine(targetPosition, adjustedPos, Color.blue);
            if (agent.CalculatePath(adjustedPos, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    return true;
                }
            }
        }
        return false;
    }

}