using UnityEngine;
using System.Collections.Generic;

public class PlayerBuffs : MonoBehaviour, IDataPersistence
{
    private List<string> activeBuffs = new List<string>();
    public List<string> ActiveBuffs => activeBuffs;

    public void AddBuff(string buffID)
    {
        if (!activeBuffs.Contains(buffID))
            activeBuffs.Add(buffID);
    }

    public bool HasBuff(string buffID)
    {
        return activeBuffs.Contains(buffID);
    }
    public void SaveData(ref GameData data)
    {
        data.buffs = activeBuffs != null
      ? activeBuffs.ToArray()
      : new string[0];
    }
    public void LoadData(GameData data)
    {
        activeBuffs = data.buffs != null
        ? new List<string>(data.buffs)
        : new List<string>();
    }
}
