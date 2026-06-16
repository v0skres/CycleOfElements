using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenuButton : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Debug.Log("Возврат в главное меню");
        SceneManager.LoadScene("MainMenu");
    }
}