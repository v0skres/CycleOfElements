using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "CycleOfElements/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int hp;
    public int baseDamage;
    public int defense;
    public EnemyAI.AIType aiType;
    public CardData.Element elementAffinity; // стихия врага (для отображения)
    public List<CardData> deck;
    [TextArea] public string specialFeature;
}