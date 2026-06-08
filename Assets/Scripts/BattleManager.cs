using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public int playerHP = 250;
    public int playerDefense = 8;
    public int playerMaxHP = 250;
    public int baseDefense = 8;

    public Enemy currentEnemy;

    public CardData.Element currentFieldElement;
    public CardData.Element previousFieldElement;

    public bool isPlayerTurn = true;
    public bool waitingForAI = false;
    private bool battleEnded = false;
    private bool endTurnRequested = false;
    private bool isResetting = false;

    public List<CardData> playerDeck;
    public List<CardData> startingDeck; // эталонная колода для сброса
    public List<CardData> playerHand = new List<CardData>();
    public List<CardData> playerDiscard = new List<CardData>();
    public int handSize = 5;

    public event System.Action OnTurnChanged;
    public event System.Action OnUIUpdate;

    public List<StatusEffect> playerStatuses = new List<StatusEffect>();
    public List<StatusEffect> enemyStatuses = new List<StatusEffect>();

    public UIManager uiManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (currentEnemy == null) { Debug.LogError("Нет врага!"); return; }
        if (playerDeck == null || playerDeck.Count == 0) { Debug.LogError("Колода пуста!"); return; }

        currentFieldElement = CardData.Element.Fire;
        previousFieldElement = CardData.Element.Fire;

        playerDefense = baseDefense;

        ShuffleDeck();
        DrawInitialHand();
        StartCoroutine(GameLoop());
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < playerDeck.Count; i++)
        {
            CardData temp = playerDeck[i];
            int rand = Random.Range(i, playerDeck.Count);
            playerDeck[i] = playerDeck[rand];
            playerDeck[rand] = temp;
        }
    }

    void DrawInitialHand()
    {
        for (int i = 0; i < handSize; i++) DrawCard();
    }

    void DrawCard()
    {
        if (playerDeck.Count == 0)
        {
            if (playerDiscard.Count > 0)
            {
                Debug.Log("Колода пуста, перетасовываем сброс");
                playerDeck = new List<CardData>(playerDiscard);
                playerDiscard.Clear();
                ShuffleDeck();
            }
            else
            {
                Debug.Log("Нет карт ни в колоде, ни в сбросе!");
                return;
            }
        }

        playerHand.Add(playerDeck[0]);
        playerDeck.RemoveAt(0);
        UpdateUI();
    }

    void DrawCardsAtEndOfTurn(int baseDrawCount)
    {
        int maxDraw = handSize - playerHand.Count;
        if (maxDraw <= 0)
        {
            Debug.Log("Рука уже полна или превышает лимит, добор карт не производится");
            return;
        }
        int cardsToDraw = Mathf.Min(baseDrawCount, maxDraw);
        Debug.Log($"Добираем {cardsToDraw} карт (из {baseDrawCount} возможных) до лимита {handSize}");
        for (int i = 0; i < cardsToDraw; i++)
        {
            DrawCard();
        }
    }

    IEnumerator GameLoop()
    {
        battleEnded = false;
        while (!battleEnded && playerHP > 0 && currentEnemy != null && currentEnemy.hp > 0)
        {
            if (isResetting) yield break;
            if (isPlayerTurn)
            {
                endTurnRequested = false;
                waitingForAI = false;
                yield return new WaitUntil(() => endTurnRequested || battleEnded || isResetting);
                if (isResetting) yield break;
                if (!battleEnded)
                {
                    DrawCardsAtEndOfTurn(2);  // вместо DrawUpToHandSize()
                    EndTurn();
                }
            }
            else
            {
                waitingForAI = true;
                yield return new WaitForSeconds(0.5f);
                if (isResetting) yield break;
                if (currentEnemy != null && !battleEnded)
                {
                    yield return StartCoroutine(currentEnemy.TakeTurn());
                }
                waitingForAI = false;
                if (isResetting) yield break;
                currentEnemy.DrawCard();
                EndTurn();
            }
        }

        if (!battleEnded && !isResetting)
        {
            if (playerHP <= 0) EndBattle(false);
            else if (currentEnemy == null || currentEnemy.hp <= 0) EndBattle(true);
        }
    }

    public void RequestEndTurn()
    {
        if (isPlayerTurn && !battleEnded)
            endTurnRequested = true;
    }

    public void PlayCard(CardDisplay cardDisplay)
    {
        Debug.Log($"PlayCard вызван, isPlayerTurn={isPlayerTurn}, battleEnded={battleEnded}");
        if (!isPlayerTurn || battleEnded) return;
        CardData card = cardDisplay.data;
        if (card == null) return;

        float modifier = CalculateDamageModifier(card.element);
        ApplyCardEffect(card, modifier);

        playerHand.Remove(card);
        playerDiscard.Add(card);
        Destroy(cardDisplay.gameObject);

        previousFieldElement = currentFieldElement;
        currentFieldElement = card.element;
        UpdateUI();

        // СМЕРТЬ ВРАГА – сразу вызываем EndBattle
        if (currentEnemy.hp <= 0)
        {
            Debug.Log("Враг мёртв, вызываем EndBattle");
            EndBattle(true);
            return; // важно – прекращаем выполнение, чтобы не было дальнейших действий
        }
    }

    public float CalculateDamageModifier(CardData.Element cardElement)
    {
        float mod = 1.0f;
        if (IsGeneration(previousFieldElement, currentFieldElement) &&
            IsGeneration(currentFieldElement, cardElement))
        {
            mod = 2.0f;
            Debug.Log($"Расширенная цепочка! x2");
        }
        else if (IsGeneration(currentFieldElement, cardElement))
        {
            mod = 1.5f;
            Debug.Log($"Порождение! x1.5");
        }

        if (IsStronger(cardElement, currentFieldElement))
        {
            mod = 2.0f;
            Debug.Log($"Сильная стихия! x2");
        }
        else if (IsWeaker(cardElement, currentFieldElement))
        {
            mod = 0.7f;
            Debug.Log($"Слабая стихия! x0.7");
        }
        return mod;
    }

    bool IsGeneration(CardData.Element from, CardData.Element to)
    {
        return (from == CardData.Element.Wood && to == CardData.Element.Fire) ||
               (from == CardData.Element.Fire && to == CardData.Element.Earth) ||
               (from == CardData.Element.Earth && to == CardData.Element.Metal) ||
               (from == CardData.Element.Metal && to == CardData.Element.Water) ||
               (from == CardData.Element.Water && to == CardData.Element.Wood);
    }

    bool IsStronger(CardData.Element card, CardData.Element field)
    {
        return (card == CardData.Element.Wood && field == CardData.Element.Earth) ||
               (card == CardData.Element.Earth && field == CardData.Element.Water) ||
               (card == CardData.Element.Water && field == CardData.Element.Fire) ||
               (card == CardData.Element.Fire && field == CardData.Element.Metal) ||
               (card == CardData.Element.Metal && field == CardData.Element.Wood);
    }

    bool IsWeaker(CardData.Element card, CardData.Element field)
    {
        return IsStronger(field, card);
    }

    public float GetDamageModifierFromWeaken(List<StatusEffect> statuses)
    {
        var weaken = statuses.Find(s => s.type == CardData.StatusEffect.Weaken);
        if (weaken != null)
            return 1 - (weaken.intensity / 100f);
        return 1f;
    }

    void ApplyCardEffect(CardData card, float modifier)
    {
        int finalDamage = Mathf.RoundToInt(card.damage * modifier);
        switch (card.type)
        {
            case CardData.CardType.Attack:
                float weakenMod = GetDamageModifierFromWeaken(playerStatuses);
                int damageWithWeaken = Mathf.RoundToInt(finalDamage * weakenMod);
                int damage = Mathf.Max(1, damageWithWeaken - currentEnemy.defense);
                currentEnemy.hp -= damage;
                Debug.Log($"Нанесено {damage} урона (ослабление: {weakenMod}). Осталось HP врага: {currentEnemy.hp}");
                break;
            case CardData.CardType.Defense:
                playerDefense += finalDamage;
                Debug.Log($"Защита увеличена на {finalDamage}, теперь {playerDefense}");
                break;
            case CardData.CardType.Status:
                if (card.statusEffect != CardData.StatusEffect.None)
                    ApplyStatusToEnemy(card.statusEffect, card.statusValue);
                break;
            case CardData.CardType.Combo:
                int comboDamage = Mathf.Max(1, finalDamage - currentEnemy.defense);
                currentEnemy.hp -= comboDamage;
                break;
        }
    }

    public void ApplyStatusToPlayer(CardData.StatusEffect type, int value)
    {
        ApplyStatus(playerStatuses, type, value);
    }

    public void ApplyStatusToEnemy(CardData.StatusEffect type, int value)
    {
        ApplyStatus(enemyStatuses, type, value);
    }

    private void ApplyStatus(List<StatusEffect> list, CardData.StatusEffect type, int value)
    {
        // Если эффект уже есть, можно усилить интенсивность или продлить длительность
        var existing = list.Find(s => s.type == type);
        if (existing != null)
        {
            existing.intensity += value;
            existing.duration = 2; // сброс длительности
        }
        else
        {
            list.Add(new StatusEffect(type, value, 2));
        }
    }


    private void ProcessStatusEffects(List<StatusEffect> statuses, bool isPlayer)
    {
        for (int i = statuses.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = statuses[i];
            switch (effect.type)
            {
                case CardData.StatusEffect.Burn:
                case CardData.StatusEffect.Poison:
                    int damage = effect.intensity;
                    if (isPlayer)
                        playerHP -= damage;
                    else
                        currentEnemy.hp -= damage;
                    Debug.Log($"{(isPlayer ? "Игрок" : "Враг")} получает {damage} урона от {effect.type}");
                    break;
                case CardData.StatusEffect.Weaken:
                    // weaken уменьшает урон на x% (реализуем в расчёте модификатора)
                    // Здесь только логирование, эффект будет применяться в CalculateDamageModifier
                    Debug.Log($"{(isPlayer ? "Игрок" : "Враг")} ослаблен: -{effect.intensity}% урона");
                    break;
                case CardData.StatusEffect.Stun:
                    // stun пропускает ход (реализуем в управлении ходами)
                    Debug.Log($"{(isPlayer ? "Игрок" : "Враг")} оглушён");
                    break;
            }
            effect.Tick();
            if (effect.IsExpired)
                statuses.RemoveAt(i);
        }
    }

    void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        if (isPlayerTurn)
        {
            playerDefense = baseDefense;
            Debug.Log($"Защита сброшена до {baseDefense}");

            ProcessStatusEffects(playerStatuses, true);
            ProcessStatusEffects(enemyStatuses, false);

            if (playerStatuses.Exists(s => s.type == CardData.StatusEffect.Stun))
            {
                Debug.Log("Игрок оглушён и пропускает ход!");
                playerStatuses.RemoveAll(s => s.type == CardData.StatusEffect.Stun);
                EndTurn();
                return;
            }
        }
        else
        {
            ProcessStatusEffects(enemyStatuses, false);
            ProcessStatusEffects(playerStatuses, true);

            if (enemyStatuses.Exists(s => s.type == CardData.StatusEffect.Stun))
            {
                Debug.Log("Враг оглушён и пропускает ход!");
                enemyStatuses.RemoveAll(s => s.type == CardData.StatusEffect.Stun);
                EndTurn();
                return;
            }
        }
        OnTurnChanged?.Invoke();
        UpdateUI();
    }

    void EndBattle(bool victory)
    {
        if (battleEnded) return;
        battleEnded = true;

        if (victory)
        {
            Debug.Log("Победа! Показываем кнопку перехода");
            if (uiManager != null)
                uiManager.ShowNextBattleButton(true);
            else
                Debug.LogError("uiManager не назначен в BattleManager");
        }
        else
        {
            Debug.Log("Поражение...");
        }
    }

    public void ResetBattleForNextEnemy()
    {
        Debug.Log("ResetBattleForNextEnemy: начало сброса");
        StopAllCoroutines(); // останавливаем старый цикл

        // Сброс флагов
        battleEnded = false;
        endTurnRequested = false;
        waitingForAI = false;
        isPlayerTurn = true;
        playerDefense = baseDefense;
        playerStatuses.Clear();
        enemyStatuses.Clear();

        // Лечение игрока
        playerHP = Mathf.Min(playerMaxHP, playerHP + Mathf.RoundToInt(playerMaxHP * 0.3f));
        playerDefense = 8;

        // Восстановление колоды
        if (startingDeck != null && startingDeck.Count > 0)
            playerDeck = new List<CardData>(startingDeck);
        playerHand.Clear();
        playerDiscard.Clear();
        ShuffleDeck();
        DrawInitialHand();

        // Сброс поля
        currentFieldElement = CardData.Element.Fire;
        previousFieldElement = CardData.Element.Fire;

        // Скрываем кнопку перехода
        if (uiManager != null)
            uiManager.ShowNextBattleButton(false);

        // Запускаем новый цикл боя
        StartCoroutine(GameLoop());

        // Обновляем UI
        OnUIUpdate?.Invoke();
        OnTurnChanged?.Invoke();
    }

    void UpdateUI() => OnUIUpdate?.Invoke();
}