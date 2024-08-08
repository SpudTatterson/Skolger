using System.Collections.Generic;
using UnityEngine;
using Sydewa;
//using UnityEngine.Events;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    //Credits to "Probably Spoonie"in youtube and his video https://www.youtube.com/watch?v=m9hj9PdO328&ab_channel=ProbablySpoonie
    //He made the original script and i've HEAVILY modified it to fit my needs. 
    //Hopefully this modified version is useful to you
    //Credits to Sydewa as i took the code from his asset and modified it

    [Header("Preset")]
    [SerializeField] LightingPreset Preset;

    [Header("Settings")]
    [SerializeField] Vector2 morningInterval = new Vector2(0f, 0.5f);
    [SerializeField] Vector2 afterNoonInterval = new Vector2(0.5f, 1f);

    [Header("Sun")]
    [SerializeField] Light SunDirectionalLight;
    [SerializeField] Vector2 lightIntensity = new Vector2(0f, 1f);
    [SerializeField] bool IsShadowChangeOn;
    [Range(0f, 1f)] public float shadowStrength = 0.5f;
    [SerializeField] float sunYRotation = 45f;

    [Header("Moon")]
    [SerializeField] bool isMoonActive = true;
    [SerializeField] Light MoonDirectionalLight;
    [SerializeField] bool IsMoonRotationOn;
    [SerializeField] float moonYRotation = 15f;
    [SerializeField] Vector2 MoonIntensity = new Vector2(0f, 1f);
    [Range(0f, 1f)] public float MoonShadowStrength = 0.5f;

    [Header("Skybox")]
    [SerializeField] bool IsSkyBoxOn;
    [SerializeField] Material skyboxMat;
    [SerializeField] string customPropertyName;

    float intensity;
    float _shadowStrength;
    float skyboxParam;

    enum RotationAxis { X, Y }
    [SerializeField] RotationAxis rotationAxis = RotationAxis.X;

    void Update()
    {
        if (Preset == null) return;

        UpdateLighting(DayNightTimeManager.Instance.TimePercent);   
    }

    void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        UpdateSunLighting(timePercent);
        UpdateMoonLighting(timePercent);
        UpdateSkybox(timePercent);
    }

    void UpdateSunLighting(float timePercent)
    {
        if (SunDirectionalLight == null) return;

        SunDirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

        Vector3 rotationEuler = rotationAxis == RotationAxis.X
            ? new Vector3((timePercent * 360f) - 90f, sunYRotation, SunDirectionalLight.transform.localRotation.z)
            : new Vector3(SunDirectionalLight.transform.localRotation.x, (timePercent * 360f) - 90f, SunDirectionalLight.transform.localRotation.z);

        SunDirectionalLight.transform.localRotation = Quaternion.Euler(rotationEuler);

        SetSunLightingIntensity(timePercent);
        SunDirectionalLight.intensity = intensity;

        if (IsShadowChangeOn)
            SunDirectionalLight.shadowStrength = _shadowStrength;
    }

    void SetSunLightingIntensity(float timePercent)
    {
        if (timePercent < morningInterval.x || timePercent > afterNoonInterval.y)
        {
            intensity = lightIntensity.x;
            _shadowStrength = 0f;
        }
        else if (timePercent >= morningInterval.x && timePercent <= morningInterval.y)
        {
            float normalizedTime = (timePercent - morningInterval.x) / (morningInterval.y - morningInterval.x);
            intensity = Mathf.Lerp(lightIntensity.x, lightIntensity.y, normalizedTime);
            _shadowStrength = Mathf.Lerp(0f, shadowStrength, normalizedTime);
        }
        else if (timePercent > morningInterval.y && timePercent < afterNoonInterval.x)
        {
            intensity = lightIntensity.y;
            _shadowStrength = shadowStrength;
        }
        else if (timePercent >= afterNoonInterval.x && timePercent <= afterNoonInterval.y)
        {
            float normalizedTime = (timePercent - afterNoonInterval.x) / (afterNoonInterval.y - afterNoonInterval.x);
            intensity = Mathf.Lerp(lightIntensity.y, lightIntensity.x, normalizedTime);
            _shadowStrength = Mathf.Lerp(0f, shadowStrength, normalizedTime);
        }
    }

    void UpdateMoonLighting(float timePercent)
    {
        if (MoonDirectionalLight == null) return;

        SetMoonLightingIntensity(timePercent);

        if (IsMoonRotationOn)
        {
            Vector3 rotationEuler = rotationAxis == RotationAxis.X
                ? new Vector3((timePercent * 360f) + 90f, moonYRotation, MoonDirectionalLight.transform.localRotation.z)
                : new Vector3(MoonDirectionalLight.transform.localRotation.x, (timePercent * 360f) + 90f, MoonDirectionalLight.transform.localRotation.z);

            MoonDirectionalLight.transform.localRotation = Quaternion.Euler(rotationEuler);
        }
    }

    void SetMoonLightingIntensity(float timePercent)
    {
        if (timePercent < morningInterval.x || timePercent > afterNoonInterval.y)
        {
            MoonDirectionalLight.intensity = MoonIntensity.y;
            MoonDirectionalLight.shadowStrength = MoonShadowStrength;
        }
        else if (timePercent >= morningInterval.x && timePercent <= morningInterval.y)
        {
            float normalizedTime = (timePercent - morningInterval.x) / (morningInterval.y - morningInterval.x);
            MoonDirectionalLight.intensity = Mathf.Lerp(MoonIntensity.y, MoonIntensity.x, normalizedTime);
            MoonDirectionalLight.shadowStrength = Mathf.Lerp(MoonShadowStrength, 1f, normalizedTime);
        }
        else if (timePercent > morningInterval.y && timePercent < afterNoonInterval.x)
        {
            MoonDirectionalLight.intensity = MoonIntensity.x;
            MoonDirectionalLight.shadowStrength = 0f;
        }
        else if (timePercent >= afterNoonInterval.x && timePercent <= afterNoonInterval.y)
        {
            float normalizedTime = (timePercent - afterNoonInterval.x) / (afterNoonInterval.y - afterNoonInterval.x);
            MoonDirectionalLight.intensity = Mathf.Lerp(MoonIntensity.x, MoonIntensity.y, normalizedTime);
            MoonDirectionalLight.shadowStrength = Mathf.Lerp(0f, MoonShadowStrength, normalizedTime);
        }
    }

    void UpdateSkybox(float timePercent)
    {
        if (!IsSkyBoxOn || skyboxMat == null) return;

        if (timePercent < morningInterval.x || timePercent > afterNoonInterval.y)
        {
            skyboxParam = 1f;
        }
        else if (timePercent >= morningInterval.x && timePercent <= morningInterval.y)
        {
            float normalizedTime = (timePercent - morningInterval.x) / (morningInterval.y - morningInterval.x);
            skyboxParam = Mathf.Lerp(1f, 0f, normalizedTime);
        }
        else if (timePercent > morningInterval.y && timePercent < afterNoonInterval.x)
        {
            skyboxParam = 0f;
        }
        else if (timePercent >= afterNoonInterval.x && timePercent <= afterNoonInterval.y)
        {
            float normalizedTime = (timePercent - afterNoonInterval.x) / (afterNoonInterval.y - afterNoonInterval.x);
            skyboxParam = Mathf.Lerp(0f, 1f, normalizedTime);
        }

        skyboxMat.SetFloat(customPropertyName, skyboxParam);
    }

    //Try to find a directional light and skybox material to use if we haven't set one
    void OnValidate()
    {
        //---------------------------Directional Light ----------------------------
        if (SunDirectionalLight != null)
            return;

        //Search for lighting tab sun
        if (RenderSettings.sun == null)
        {
            SunDirectionalLight = RenderSettings.sun;
        }
        //Search scene for light that fits criteria (directional)
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    SunDirectionalLight = light;
                    return;
                }
            }
        }

        //--------------------------Skybox-------------------------------

        if (skyboxMat != null)
            return;

        if (RenderSettings.skybox != null)
        {
            skyboxMat = RenderSettings.skybox;
        }

        //------Moon
        if (isMoonActive && MoonDirectionalLight != null)
        {
            UpdateMoonLighting(DayNightTimeManager.Instance.TimeOfDay / 24f);
        }
    }
}

