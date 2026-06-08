using UnityEngine;
using UnityEngine.UI;
using TMPro;

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