using System;
using System.IO;
using UnityEngine;

[Serializable]
public class GameSettingsData{
    public float master = 1f;
    public float music = 1f;
    public int qualityInd = 0;
    public bool fullscreen = true;
    public int resWidth = 1920;
    public int resHeight = 1080;
}

public static class GameSettingsStore{
    private const string FileName = "settings.json";

    public static string PathToFile => System.IO.Path.Combine(Application.persistentDataPath, FileName);

    public static GameSettingsData Load(){
        try{
            if (!(File.Exists(PathToFile)))
                return new GameSettingsData();

            string json = File.ReadAllText(PathToFile);
            var data = JsonUtility.FromJson<GameSettingsData>(json);
            return data ?? new GameSettingsData();
        }
        catch (Exception e){
            Debug.LogWarning($"Settings not loaded {(e.Message)}");
            return new GameSettingsData();
        }
    }

    public static void Save(GameSettingsData data){
        try {
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(PathToFile, json);
        }
        catch (Exception e) {
            Debug.LogWarning($"Settings not saved {(e.Message)}");
        }
    }
}
