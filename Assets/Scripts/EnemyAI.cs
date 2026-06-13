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

        if (bm.uiManager != null)
            bm.uiManager.ShowEnemyCard(chosenCard.cardName);

        float modifier = bm.CalculateDamageModifier(chosenCard.element);

        if (bm.uiManager != null)
        {
            string comboMsg = "";
            if (Mathf.Approximately(modifier, 2.0f))
                comboMsg = "ПРЕОДОЛЕНИЕ x2!";
            else if (Mathf.Approximately(modifier, 1.5f))
                comboMsg = "ПОРОЖДЕНИЕ x1.5!";
            else if (Mathf.Approximately(modifier, 0.7f))
                comboMsg = "СЛАБАЯ СТИХИЯ x0.7!";
            if (!string.IsNullOrEmpty(comboMsg))
                bm.uiManager.ShowEnemyCombo(comboMsg);
        }

        float weakenMod = BattleManager.Instance.GetDamageModifierFromWeaken(BattleManager.Instance.enemyStatuses);
        int finalDamage = Mathf.RoundToInt(chosenCard.damage * modifier * weakenMod);

        switch (chosenCard.type)
        {
            case CardData.CardType.Attack:
                int dmg = Mathf.Max(1, finalDamage - bm.playerDefense);
                bm.playerHP -= dmg;
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.playerHit);
                CameraShake.Instance?.Shake(0.2f, 0.1f);
                bm.uiManager?.FlashRed();
                Debug.Log($"Враг нанёс {dmg} урона");
                if (bm.playerHP <= 0)
                {
                    Debug.Log("Игрок мёртв, вызываем EndBattle");
                    bm.EndBattle(false);
                }
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