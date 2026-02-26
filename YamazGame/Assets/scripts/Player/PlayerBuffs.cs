using UnityEngine;
using System.Collections.Generic;

public class PlayerBuffs : MonoBehaviour
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
}
