using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public GameObject youDiedText; // Objekt textu, který zobrazuje "You Died"

    public GameObject mainMenuButton; // Tlačítko pro hlavní menu

    public GameObject retryButton; // Tlačítko pro opakování

    public GameObject crosshair; // Objekt zaměřovače (crosshair)

    public AudioSource[] playerAudioSources; // Array of player's Audio Sources

    private void Start()
    {
        // Na začátku hry skryjeme tlačítka Retry a MainMenu
        ShowHide(false);
    }

    // Funkce pro zobrazení/skrytí tlačítek Retry a MainMenu
    public void ShowHide(bool show)
    {
        youDiedText.SetActive (show);
        retryButton.SetActive (show);
        mainMenuButton.SetActive (show); // Tato řádka skryje/zobrazí tlačítko MainMenu spolu s tlačítkem Retry
        crosshair.SetActive(!show); // Tato řádka skryje/zobrazí zaměřovač (crosshair) opačně
    }

    public void OnRetryButtonPressed()
    {
        // Restartujeme scénu
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Obnovíme časovou škálu
        Time.timeScale = 1;

        // Resetujeme proměnnou slotFull
        PickUpController.slotFull = false;

        // Skryjeme tlačítka Retry a MainMenu
        ShowHide(false);

        // Povolíme Audio Sources hráče
        foreach (AudioSource audioSource in playerAudioSources)
        {
            audioSource.enabled = true;
        }
    }

    public void OnMainMenuButtonPressed()
    {
        // Načteme hlavní menu
        SceneManager.LoadScene("Main Menu");

        // Obnovíme časovou škálu
        Time.timeScale = 1;

        // Povolíme Audio Sources hráče
        foreach (AudioSource audioSource in playerAudioSources)
        {
            audioSource.enabled = true;
        }
    }
}
