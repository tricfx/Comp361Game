using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider Master;
    public Slider Music;
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    void Start()
    {
        // Build unique list (width x height only)
        var unique = new Dictionary<string, Resolution>();
        foreach (var r in Screen.resolutions)
        {
            string key = $"{r.width}x{r.height}";
            if (!unique.ContainsKey(key))
                unique[key] = r;
        }

        var list = unique.Values
            .OrderBy(r => r.width)
            .ThenBy(r => r.height)
            .ToList();

        resolutions = list.ToArray();

        // Fill dropdown
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutions.Select(r => $"{r.width} x {r.height}").ToList());

        // Set dropdown to current resolution
        int currentIndex = System.Array.FindIndex(resolutions,
            r => r.width == Screen.width && r.height == Screen.height);

        if (currentIndex < 0) currentIndex = 0;

        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();

        // IMPORTANT: hook listener (so selecting applies)
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        // Optional: apply the default immediately (keeps things consistent)
         SetResolution(currentIndex);
    }

    public void SetResolution(int index)
    {
        if (index < 0 || index >= resolutions.Length) return;

        Resolution r = resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Master.value);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music", Music.value);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Debug.Log("Setting fullscreen: " + isFullscreen);
        Screen.fullScreen = isFullscreen;


    }
}
