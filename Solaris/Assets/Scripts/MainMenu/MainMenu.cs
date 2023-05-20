using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Scene";  // přidáno

    public void PlayGame()  // Veřejná funkce bez parametrů
    {
        PickUpController.slotFull = false;
        SceneManager.LoadScene(gameSceneName); // název scény je nyní ovládán proměnnou
    }

    public void QuitGame()  // Veřejná funkce bez parametrů
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
