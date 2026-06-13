using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
    }
}