using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public float displayTime = 3f;

    void Awake() { Instance = this; }

    public void ShowDialogue(string text, float duration = -1)
    {
        if (duration < 0) duration = displayTime;
        StartCoroutine(ShowMessage(text, duration));
    }

    IEnumerator ShowMessage(string text, float duration)
    {
        dialogueText.text = text;
        dialoguePanel.SetActive(true);
        yield return new WaitForSeconds(duration);
        dialoguePanel.SetActive(false);
    }
}