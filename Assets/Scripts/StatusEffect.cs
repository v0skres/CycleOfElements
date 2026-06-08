using System;

[Serializable]
public class StatusEffect
{
    public CardData.StatusEffect type;
    public int intensity;
    public int duration;

    public StatusEffect(CardData.StatusEffect type, int intensity, int duration = 2)
    {
        this.type = type;
        this.intensity = intensity;
        this.duration = duration;
    }

    public void Tick() => duration--;
    public bool IsExpired => duration <= 0;
}