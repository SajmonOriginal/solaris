using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public GameObject youDiedText;
    public GameObject mainMenuButton; // Drag the MainMenu button UI here
    public GameObject retryButton; // Drag the Retry button UI here
    public GameObject crosshair; // Drag your Crosshair object here

    public AudioSource[] playerAudioSources; // Array of player's Audio Sources

    private void Start()
    {
        // Hide the Retry and MainMenu buttons at the start of the game
        ShowHide(false);
    }

    public void ShowHide(bool show)
    {
        youDiedText.SetActive(show);
        retryButton.SetActive(show);
        mainMenuButton.SetActive(show); // This line will hide/show the MainMenu button with the Retry button
        crosshair.SetActive(!show); // this line will hide/show the crosshair inversely
    }

    public void OnRetryButtonPressed()
    {
        // Restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Reset the timescale
        Time.timeScale = 1;

        // reset slotFull
        PickUpController.slotFull = false;

        // Hide the Retry and MainMenu buttons
        ShowHide(false);

        // Enable player's Audio Sources
        foreach (AudioSource audioSource in playerAudioSources)
        {
            audioSource.enabled = true;
        }
    }

    public void OnMainMenuButtonPressed()
    {
        // Load the main menu
        SceneManager.LoadScene("Main Menu");

        // Reset the timescale
        Time.timeScale = 1;

        // Enable player's Audio Sources
        foreach (AudioSource audioSource in playerAudioSources)
        {
            audioSource.enabled = true;
        }
    }
}
