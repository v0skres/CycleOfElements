using UnityEngine;

public class WorldMapIntro : MonoBehaviour
{
    private const string PROLOGUE_SHOWN_KEY = "PrologueShown";

    void Start()
    {
        // Показываем пролог только если его ещё не видели в этой сессии (или вообще)
        if (!PlayerPrefs.HasKey(PROLOGUE_SHOWN_KEY) || PlayerPrefs.GetInt(PROLOGUE_SHOWN_KEY) == 0)
        {
            ShowPrologue();
            PlayerPrefs.SetInt(PROLOGUE_SHOWN_KEY, 1);
            PlayerPrefs.Save();
        }
    }

    void ShowPrologue()
    {
        string prologueText = "«Кристалл Баланса разрушен. Пять осколков разошлись по миру. Ты должен собрать их, пока хаос не поглотил всё. Начни с Огненного клана.»";
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowDialogue(prologueText, 5f);
        else
            Debug.LogWarning("DialogueManager.Instance не найден!");
    }

    // Опционально: кнопка "История" на карте мира может вызывать этот метод повторно
    public void ShowPrologueAgain()
    {
        ShowPrologue();
    }
}