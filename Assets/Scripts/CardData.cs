using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "CycleOfElements/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public enum Element { Fire, Water, Earth, Metal, Wood }
    public Element element;
    public enum CardType { Attack, Defense, Status, Combo }
    public CardType type;
    public int damage;          // базовый урон/защита
    public string description;
    public Sprite cardImage;
    public enum StatusEffect { None, Burn, Poison, Weaken, Stun }
    public StatusEffect statusEffect;
    public int statusValue;     // сила эффекта (например, 5 урона от отравления)
}