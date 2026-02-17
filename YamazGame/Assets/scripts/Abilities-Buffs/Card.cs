using UnityEngine;

public abstract class Card : ScriptableObject
{
    public string cardID;
    public string cardName;
    public string cardDescription;
    public Sprite icon;

    public CardType type;

    public abstract void Apply(Player player);
}