using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int hp;
    public int baseDamage;
    public int defense;
    public EnemyAI.AIType aiType;
    public List<CardData> deck;
    public List<CardData> hand = new List<CardData>();
    public List<CardData> discard = new List<CardData>();
    public int baseDefense;

    public void InitializeFromData(EnemyData data)
    {
        enemyName = data.enemyName;
        hp = data.hp;
        baseDamage = data.baseDamage;
        baseDefense = data.defense;   // запоминаем базовую защиту
        defense = baseDefense;
        aiType = data.aiType;
        deck = new List<CardData>(data.deck);
        hand.Clear();
        discard.Clear();
        // Начальная рука: 3 карты
        for (int i = 0; i < 3 && deck.Count > 0; i++)
        {
            hand.Add(deck[0]);
            deck.RemoveAt(0);
        }
    }

    public void DrawCard()
    {
        if (deck.Count == 0)
        {
            if (discard.Count > 0)
            {
                deck = new List<CardData>(discard);
                discard.Clear();
                Shuffle(deck);
            }
            else return;
        }
        hand.Add(deck[0]);
        deck.RemoveAt(0);
    }

    private void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CardData temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    public CardData GetRandomCardFromHand()
    {
        if (hand.Count == 0) return null;
        return hand[Random.Range(0, hand.Count)];
    }

    public void RemoveCardFromHand(CardData card)
    {
        hand.Remove(card);
        discard.Add(card);
    }

    public void ApplyStatus(CardData.StatusEffect effect, int value)
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.ApplyStatusToEnemy(effect, value);
    }

    public IEnumerator TakeTurn()
    {
        // Сброс защиты до базовой в начале хода врага
        defense = baseDefense;
        Debug.Log($"Защита врага {enemyName} сброшена до {defense}");

        yield return EnemyAI.ExecuteTurn(this);
    }
}