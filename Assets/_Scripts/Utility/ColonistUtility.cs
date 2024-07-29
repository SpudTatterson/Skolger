using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColonistUtility
{
    public static Sprite CaptureFace(GameObject objectToCapture, float faceHeight, Vector3 offset, int width, int height)
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
        captureCamera.farClipPlane = 2.5f;

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
}
