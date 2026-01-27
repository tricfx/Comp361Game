using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Slider Master;
    public Slider Music;
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;   
    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master",Master.value);
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
        Screen.fullScreen = isFullscreen;
    }


}
