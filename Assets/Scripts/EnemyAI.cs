using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyAI
{
    public enum AIType { Aggressive, Defensive, Tactical, Adaptive }

    public static IEnumerator ExecuteTurn(Enemy enemy)
    {
        BattleManager bm = BattleManager.Instance;
        if (bm == null || enemy == null) yield break;

        CardData chosenCard = enemy.GetRandomCardFromHand();
        if (chosenCard == null)
        {
            Debug.Log("У врага нет карт, ход пропущен");
            yield break;
        }

        float modifier = bm.CalculateDamageModifier(chosenCard.element);
        float weakenMod = BattleManager.Instance.GetDamageModifierFromWeaken(BattleManager.Instance.enemyStatuses);
        int finalDamage = Mathf.RoundToInt(chosenCard.damage * modifier * weakenMod);

        switch (chosenCard.type)
        {
            case CardData.CardType.Attack:
                int dmg = Mathf.Max(1, finalDamage - bm.playerDefense);
                bm.playerHP -= dmg;
                Debug.Log($"Враг нанёс {dmg} урона");
                break;
            case CardData.CardType.Defense:
                enemy.defense += finalDamage;
                Debug.Log($"Враг увеличил защиту на {finalDamage}, теперь {enemy.defense}");
                break;
            default:
                Debug.Log($"Враг использовал {chosenCard.cardName}");
                break;
            case CardData.CardType.Status:
                if (chosenCard.statusEffect != CardData.StatusEffect.None)
                {
                    BattleManager.Instance.ApplyStatusToPlayer(chosenCard.statusEffect, chosenCard.statusValue);
                    Debug.Log($"Враг наложил статус {chosenCard.statusEffect} на игрока");
                }
                break;
        }

        enemy.RemoveCardFromHand(chosenCard);
        bm.previousFieldElement = bm.currentFieldElement;
        bm.currentFieldElement = chosenCard.element;
        yield return new WaitForSeconds(1f);
    }
}