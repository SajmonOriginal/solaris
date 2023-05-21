using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public GameObject youWinText; // Textový objekt UI zobrazující "Vyhrál jsi"
    public GameObject nextButton; // Tlačítko UI pro přechod na další úroveň
    public GameObject menuButton; // Tlačítko UI pro návrat do hlavního menu
    public GameObject crosshair; // Objekt představující zaměřovač

    public AudioSource[] playerAudioSources; // Pole Audio Source hráče

    private void Start()
    {
        // Na začátku hry skryjeme text "Vyhrál jsi", tlačítko "Další" a tlačítko "Menu"
        ShowHide(false);
    }

    // Metoda pro zobrazení/skrytí prvků UI
    public void ShowHide(bool show)
    {
        youWinText.SetActive(show);
        nextButton.SetActive(show);
        menuButton.SetActive(show);
        crosshair.SetActive(!show); // Tato řádka skrývá/zobrazuje zaměřovač opačně
    }

    // Metoda volaná po stisknutí tlačítka "Další"
    public void OnNextButtonPressed()
    {
        // Získáme název aktuální úrovně
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Načteme další úroveň na základě aktuální úrovně
        if (currentSceneName == "Level1")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (currentSceneName == "Level2")
        {
            SceneManager.LoadScene("Level1");
        }

        // Resetujeme časovou škálu
        Time.timeScale = 1;

        // Povolíme Audio Source hráče
        foreach (AudioSource audioSource in playerAudioSources)
        {
            audioSource.enabled = true;
        }

        // Resetujeme stav vybavení zbraní hráče
        PickUpController.slotFull = false;
    }

    // Metoda volaná po stisknutí tlačítka "Menu"
    public void OnMenuButtonPressed()
    {
        // Načteme hlavní menu
        SceneManager.LoadScene("Main Menu");

        // Resetujeme časovou škálu
        Time.timeScale = 1;

        // Povolíme Audio Source hráče
        foreach (AudioSource audioSource in playerAudioSources)
        {
            audioSource.enabled = true;
        }

        // Resetujeme stav vybavení zbraní hráče
        PickUpController.slotFull = false;
    }
}
