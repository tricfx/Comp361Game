using UnityEngine;

public abstract class Card : ScriptableObject
{
    public string cardID;
    public string cardName;
    public string cardDescription;
    public Sprite icon;
    public string dependency;
    public int cooldownSeconds;
    public CardType type;
    [Min(0)] public int shopCost;

    public abstract void Apply(GameObject playerObject);
}