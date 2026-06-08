using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public CardData data;
    public TMP_Text nameText;
    public TMP_Text damageText;
    public Image background;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnCardClick);
            Debug.Log($"CardDisplay: кнопка {name} подписана");
        }
        else
            Debug.LogError($"CardDisplay: нет компонента Button на {name}");
    }

    public void Setup(CardData cardData)
    {
        data = cardData;
        nameText.text = cardData.cardName;
        damageText.text = cardData.damage.ToString();
        // можно также покрасить фон в цвет стихии
    }

    public void OnCardClick()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.PlayCard(this);
        }
        else
        {
            Debug.LogError("BattleManager.Instance не найден!");
        }
    }
}