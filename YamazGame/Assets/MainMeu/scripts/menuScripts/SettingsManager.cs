using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider master;
    [SerializeField] private Slider music;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterParam = "Master";
    [SerializeField] private string musicParam = "Music";

    private Resolution[] resolutions;

    void Start()
    {
        BuildResolutionDropdown();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        if (master) master.onValueChanged.AddListener(SetMasterVolume);
        if (music) music.onValueChanged.AddListener(SetMusicVolume);

        if (fullscreenToggle) fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        if (qualityDropdown) qualityDropdown.onValueChanged.AddListener(SetQuality);

        if (master) SetMasterVolume(master.value);
        if (music) SetMusicVolume(music.value);
        if (fullscreenToggle) SetFullscreen(fullscreenToggle.isOn);
        if (qualityDropdown) SetQuality(qualityDropdown.value);
    }

    private void BuildResolutionDropdown()
    {
        var unique = new Dictionary<string, Resolution>();
        foreach (var r in Screen.resolutions)
        {
            string key = $"{r.width}x{r.height}";
            if (!unique.ContainsKey(key))
                unique[key] = r;
        }

        var list = unique.Values.OrderBy(r => r.width).ThenBy(r => r.height).ToList();
        resolutions = list.ToArray();

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutions.Select(r => $"{r.width} x {r.height}").ToList());

        int currentIndex = Array.FindIndex(resolutions, r => r.width == Screen.width && r.height == Screen.height);
        if (currentIndex < 0) currentIndex = 0;

        resolutionDropdown.SetValueWithoutNotify(currentIndex);
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        if (index < 0 || index >= resolutions.Length) return;
        var r = resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }

    public void SetMasterVolume(float value01)
    {
        audioMixer.SetFloat(masterParam, ToDb(value01));
    }

    public void SetMusicVolume(float value01)
    {
        float db = ToDb(value01);
        audioMixer.SetFloat(musicParam, db);
    }


    private static float ToDb(float value01)
    {
        value01 = Mathf.Clamp(value01, 0.0001f, 1f);
        return Mathf.Log10(value01) * 20f;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
