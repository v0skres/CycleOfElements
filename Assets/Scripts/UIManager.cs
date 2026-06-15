using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public TMP_Text playerHPText;
    public TMP_Text enemyHPText;
    public TMP_Text currentFieldText;
    public TMP_Text previousFieldText;
    public Button endTurnButton;
    public Transform handPanel;
    public GameObject cardPrefab;
    public TMP_Text enemyNameText;
    public Button nextBattleButton;

    public TMP_Text defenseText;          // защита игрока
    public TMP_Text enemyDefenseText;     // защита врага
    public TMP_Text playerComboText;   // левый нижний/верхний угол
    public TMP_Text enemyComboText;    // правый нижний/верхний угол

    public TMP_Text enemyCardText;

    public TMP_Text turnTimerText;

    public TMP_Text playerStatusText;
    public TMP_Text enemyStatusText;

    public Image damageOverlay;

    public GameOverUI gameOverUI;

    private bool isSubscribed = false;

    void Awake()
    {
        if (BattleManager.Instance != null) Subscribe();
    }

    void Start()
    {
        if (!isSubscribed && BattleManager.Instance != null) Subscribe();
        if (endTurnButton != null)
            endTurnButton.onClick.AddListener(() => BattleManager.Instance.RequestEndTurn());
        UpdateUI();

        if (nextBattleButton != null)
        {
            nextBattleButton.gameObject.SetActive(false);
            nextBattleButton.onClick.AddListener(OnNextBattleClicked);
        }

        if (playerComboText != null) playerComboText.gameObject.SetActive(false);
        if (enemyComboText != null) enemyComboText.gameObject.SetActive(false);

        if (enemyCardText != null)
            enemyCardText.gameObject.SetActive(false);
    }

    void Subscribe()
    {
        if (isSubscribed) return;
        var bm = BattleManager.Instance;
        if (bm != null)
        {
            bm.OnUIUpdate += UpdateUI;
            bm.OnTurnChanged += OnTurnChange;
            isSubscribed = true;
        }
    }

    public void UpdateEnemyName(string name)
    {
        if (enemyNameText != null)
            enemyNameText.text = name;
    }

    void UpdateUI()
    {
        var bm = BattleManager.Instance;
        if (bm == null) return;

        if (playerHPText != null) playerHPText.text = $"HP: {bm.playerHP}";
        if (enemyHPText != null && bm.currentEnemy != null) enemyHPText.text = $"HP: {bm.currentEnemy.hp}";
        if (currentFieldText != null) currentFieldText.text = bm.currentFieldElement.ToString();
        if (previousFieldText != null) previousFieldText.text = bm.previousFieldElement.ToString();

        // Отрисовка руки
        if (handPanel != null && cardPrefab != null && bm.playerHand != null)
        {
            foreach (Transform child in handPanel) Destroy(child.gameObject);
            foreach (CardData card in bm.playerHand)
            {
                GameObject cardGO = Instantiate(cardPrefab, handPanel);
                CardDisplay display = cardGO.GetComponent<CardDisplay>();
                if (display != null) display.Setup(card);
            }
        }

        // Защита
        if (defenseText != null)
            defenseText.text = $"Защита: {bm.playerDefense}";
        if (enemyDefenseText != null && bm.currentEnemy != null)
            enemyDefenseText.text = $"Защита: {bm.currentEnemy.defense}";
    }

    public void ShowPlayerCombo(string message, float duration = 1.5f)
    {
        if (playerComboText == null) return;
        playerComboText.text = message;
        playerComboText.gameObject.SetActive(true);
        CancelInvoke(nameof(HidePlayerCombo));
        Invoke(nameof(HidePlayerCombo), duration);
    }

    void HidePlayerCombo()
    {
        if (playerComboText != null) playerComboText.gameObject.SetActive(false);
    }

    public void ShowEnemyCombo(string message, float duration = 1.5f)
    {
        if (enemyComboText == null) return;
        enemyComboText.text = message;
        enemyComboText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideEnemyCombo));
        Invoke(nameof(HideEnemyCombo), duration);
    }

    void HideEnemyCombo()
    {
        if (enemyComboText != null) enemyComboText.gameObject.SetActive(false);
    }

    public void ShowEnemyCard(string cardName, float duration = 1.5f)
    {
        if (enemyCardText == null) return;
        enemyCardText.text = $"Враг использовал: {cardName}";
        enemyCardText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideEnemyCard));
        Invoke(nameof(HideEnemyCard), duration);
    }

    void HideEnemyCard()
    {
        if (enemyCardText != null)
            enemyCardText.gameObject.SetActive(false);
    }

    public void UpdateTurnTimer(float seconds)
    {
        if (turnTimerText != null)
        {
            int secs = Mathf.CeilToInt(seconds);
            turnTimerText.text = $"Ход: {secs} с";
            // можно подсвечивать красным, если мало времени
            if (secs <= 3) turnTimerText.color = Color.red;
            else if (secs <= 5) turnTimerText.color = Color.yellow;
            else turnTimerText.color = Color.white;
        }
    }

    public void FlashRed()
    {
        if (damageOverlay == null) return;
        damageOverlay.color = new Color(1, 0, 0, 0.5f);
        CancelInvoke(nameof(HideOverlay));
        Invoke(nameof(HideOverlay), 0.3f);
    }

    void HideOverlay()
    {
        if (damageOverlay != null)
            damageOverlay.color = new Color(1, 0, 0, 0);
    }

    public void UpdatePlayerStatusIcons(List<StatusEffect> statuses)
    {
        UpdateStatusIcons(playerStatusText, statuses);
    }

    public void UpdateEnemyStatusIcons(List<StatusEffect> statuses)
    {
        UpdateStatusIcons(enemyStatusText, statuses);
    }

    private void UpdateStatusIcons(TMP_Text text, List<StatusEffect> statuses)
    {
        if (text == null) return;
        string icons = "";
        foreach (var effect in statuses)
        {
            if (effect.type == CardData.StatusEffect.Burn) icons += "Горение";
            else if (effect.type == CardData.StatusEffect.Poison) icons += "Отравлен";
            else if (effect.type == CardData.StatusEffect.Weaken) icons += "Ослаблен";
            // Stun не показываем постоянно
        }
        text.text = icons.Trim();
    }

    public void ShowStunMessage(bool isPlayer)
    {
        string message = isPlayer ? "Вы оглушёны!" : "Враг оглушён!";
        if (isPlayer)
            ShowPlayerCombo(message, 1.5f);
        else
            ShowEnemyCombo(message, 1.5f);
    }

    public void ShowGameOver()
    {
        Debug.Log("ShowGameOver вызван");
        if (gameOverUI == null)
        {
            gameOverUI = FindObjectOfType<GameOverUI>();
            if (gameOverUI == null)
            {
                Debug.LogError("GameOverUI не найден в сцене! Создайте объект с компонентом GameOverUI.");
                return;
            }
        }
        gameOverUI.ShowGameOver();
    }

    void OnNextBattleClicked()
    {
        Debug.Log("Нажата кнопка перехода к следующему врагу");
        if (nextBattleButton != null)
            nextBattleButton.gameObject.SetActive(false);

        BattleProgression progression = FindObjectOfType<BattleProgression>();
        if (progression != null)
            progression.ProceedToNextEnemy();
        else
            Debug.LogError("BattleProgression не найден");
    }

    public void ShowNextBattleButton(bool show)
    {
        if (nextBattleButton != null)
            nextBattleButton.gameObject.SetActive(show);
    }

    void OnTurnChange()
    {
        Debug.Log($"OnTurnChange: isPlayerTurn={BattleManager.Instance.isPlayerTurn}, endTurnButton={endTurnButton != null}");
        if (endTurnButton != null && BattleManager.Instance != null)
            endTurnButton.interactable = BattleManager.Instance.isPlayerTurn;
    }

    void OnDestroy()
    {
        if (BattleManager.Instance != null && isSubscribed)
        {
            BattleManager.Instance.OnUIUpdate -= UpdateUI;
            BattleManager.Instance.OnTurnChanged -= OnTurnChange;
        }
    }
}