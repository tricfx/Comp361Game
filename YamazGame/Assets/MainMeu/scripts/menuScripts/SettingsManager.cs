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
    private GameSettingsData data;
    private SettingsManager instance;

    void Awake()
    {
        if (instance != null && instance != this){
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        //Debug.Log(GameSettingsStore.PathToFile);
        data = GameSettingsStore.Load();
    }

    void Start(){
        BuildResolutionDropdown();
        ApplyToGame(data);
        ApplyToUI(data);

        if (resolutionDropdown) resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        if (master) master.onValueChanged.AddListener(OnMasterChanged);
        if (music) music.onValueChanged.AddListener(OnMusicChanged);
        if (fullscreenToggle) fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        if (qualityDropdown) qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    private void ApplyToGame(GameSettingsData d){
        SetMasterVolume(d.master);
        SetMusicVolume(d.music);
        SetQuality(d.qualityInd);
        SetFullscreen(d.fullscreen);

        Screen.SetResolution(d.resWidth, d.resHeight, d.fullscreen);
    }

    private void ApplyToUI(GameSettingsData d){
        if (master) master.SetValueWithoutNotify(d.master);
        if (music) music.SetValueWithoutNotify(d.music);

        if (qualityDropdown){
            int q = Mathf.Clamp(d.qualityInd, 0, QualitySettings.names.Length - 1);
            qualityDropdown.SetValueWithoutNotify(q);
            qualityDropdown.RefreshShownValue();
        }

        if (fullscreenToggle) fullscreenToggle.SetIsOnWithoutNotify(d.fullscreen);

        if (resolutionDropdown){
            int resInd = FindResolutionIndex(d.resWidth, d.resHeight);
            if (resInd < 0) resInd = FindResolutionIndex(Screen.width, Screen.height);
            if (resInd < 0) resInd = 0;

            resolutionDropdown.SetValueWithoutNotify(resInd);
            resolutionDropdown.RefreshShownValue();
        }
    }

    private int FindResolutionIndex(int w, int h){
        if (resolutions == null) return -1;
        return Array.FindIndex(resolutions, r => r.width == w && r.height == h);
    }

    private void BuildResolutionDropdown(){
        var unique = new Dictionary<string, Resolution>();
        foreach (var r in Screen.resolutions){
            string key = $"{r.width}x{r.height}";
            if (!unique.ContainsKey(key)) unique[key] = r;
        }

        var list = unique.Values.OrderBy(r => r.width).ThenBy(r => r.height).ToList();
        resolutions = list.ToArray();

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutions.Select(r => $"{r.width} x {r.height}").ToList());
    }

    private void OnResolutionChanged(int index){
        if (index < 0 || index >= resolutions.Length) return;

        var r = resolutions[index];
        data.resWidth = r.width;
        data.resHeight = r.height;

        Screen.SetResolution(r.width, r.height, data.fullscreen);
        GameSettingsStore.Save(data);
    }

    private void OnMasterChanged(float value){
        data.master = value;
        SetMasterVolume(value);
        GameSettingsStore.Save(data);
    }

    private void OnMusicChanged(float value){
        data.music = value;
        SetMusicVolume(value);
        GameSettingsStore.Save(data);
    }

    private void OnQualityChanged(int qualityIndex){
        data.qualityInd = qualityIndex;
        SetQuality(qualityIndex);
        GameSettingsStore.Save(data);
    }

    private void OnFullscreenChanged(bool isFullscreen){
        data.fullscreen = isFullscreen;
        SetFullscreen(isFullscreen);
        Screen.SetResolution(data.resWidth, data.resHeight, isFullscreen);
        GameSettingsStore.Save(data);
    }
    public void SetMasterVolume(float value){
        audioMixer.SetFloat(masterParam, ToDb(value));
    }

    public void SetMusicVolume(float value){
        audioMixer.SetFloat(musicParam, ToDb(value));
    }

    private static float ToDb(float value){
        value = Mathf.Clamp(value, 0.0001f, 1f);
        return Mathf.Log10(value) * 20f;
    }

    public void SetQuality(int qualityIndex){
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen){
        Screen.fullScreen = isFullscreen;
    }
}
