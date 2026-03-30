using UnityEngine;
using System.Collections.Generic;

public class PlayerBuffs : MonoBehaviour, IDataPersistence
{
    [SerializeField] private BuffDatabase buffDatabase;

    private List<string> activeBuffs = new List<string>();
    public List<string> ActiveBuffs => activeBuffs;

    private bool buffsAppliedFromLoad = false;

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

        if (!buffsAppliedFromLoad)
        {
            ReapplyBuffs();
            buffsAppliedFromLoad = true;
        }
    }

    private void ReapplyBuffs()
    {
        foreach (string buffID in activeBuffs)
        {
            BuffCard buff = GetBuffByID(buffID);

            if (buff != null)
            {
                Debug.Log("Applying buff: " + buffID);
                buff.Apply(gameObject);
            }
            else
            {
                Debug.LogWarning("Buff not found for ID: " + buffID);
            }
        }
    }

    private BuffCard GetBuffByID(string buffID)
    {
        if (buffDatabase == null)
        {
            Debug.LogError("BuffDatabase is not assigned!");
            return null;
        }

        if (buffDatabase.allBuffCards == null || buffDatabase.allBuffCards.Length == 0)
        {
            Debug.LogError("BuffDatabase has no buff cards assigned!");
            return null;
        }

        foreach (BuffCard buff in buffDatabase.allBuffCards)
        {
            if (buff == null) continue;

            if (buff.buffID == buffID)
                return buff;
        }

        return null;
    }
}