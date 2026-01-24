using UnityEngine;

public interface IDataPersistence
{
    void LoadData(gameData data);
    void SaveData(ref gameData data);
}
