using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Scene";  // Název scény, která se načte po kliknutí na tlačítko Play

    // Funkce, která se volá po kliknutí na tlačítko Play
    public void PlayGame()
    {
        // Nastavíme hodnotu slotFull na false
        PickUpController.slotFull = false;

        // Načteme scénu s názvem gameSceneName
        SceneManager.LoadScene(gameSceneName);
    }

    // Funkce, která se volá po kliknutí na tlačítko Quit
    public void QuitGame()
    {
        // Vypíšeme zprávu "Quit!" do konzole
        Debug.Log("Quit!");

        // Ukončíme aplikaci
        Application.Quit();
    }
}
