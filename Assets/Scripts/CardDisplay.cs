using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public CardData data;
    public TMP_Text nameText;
    public TMP_Text damageText;
    public Image background;
    public Image cardArt;

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
        if (cardArt != null && cardData.cardImage != null)
            cardArt.sprite = cardData.cardImage;
    }

    public void OnCardClick()
    {
        if (BattleManager.Instance != null && BattleManager.Instance.isPlayerTurn && !BattleManager.Instance.IsBattleEnded)
        {
            AnimateCard(() => BattleManager.Instance.PlayCard(this));
        }
    }

    public void AnimateCard(System.Action onComplete)
    {
        StartCoroutine(AnimateCardRoutine(onComplete));
    }

    private IEnumerator AnimateCardRoutine(System.Action onComplete)
    {
        Vector3 originalPos = transform.localPosition;
        Vector3 targetPos = originalPos + Vector3.up * 130f;
        float duration = 0.15f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(originalPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPos;
        if (onComplete != null) onComplete();
    }
}