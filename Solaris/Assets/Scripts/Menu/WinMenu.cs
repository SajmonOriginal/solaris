using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public GameObject youWinText;
    public GameObject nextButton; // Drag the Next button UI here
    public GameObject menuButton; // Drag the Menu button UI here
    public GameObject crosshair; // Drag your Crosshair object here

    public AudioSource[] playerAudioSources; // Array of player's Audio Sources

    private void Start()
    {
        // Hide the You Win text, Next and Menu buttons at the start of the game
        ShowHide(false);
    }

    public void ShowHide(bool show)
    {
        youWinText.SetActive(show);
        nextButton.SetActive(show);
        menuButton.SetActive(show);
        crosshair.SetActive(!show); // this line will hide/show the crosshair inversely
    }

    public void OnNextButtonPressed()
    {
        // Get the current scene
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Load the next scene based on the current scene
        if (currentSceneName == "Level1")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (currentSceneName == "Level2")
        {
            SceneManager.LoadScene("Level1");
        }

            // // Try to extract the level number from the scene name
    // if (currentSceneName.StartsWith("Level") && int.TryParse(currentSceneName.Substring(5), out int currentLevel))
    // {
    //     // Load the next level
    //     string nextLevelName = "Level" + (currentLevel + 1);
    //     SceneManager.LoadScene(nextLevelName);
    // }

        // Reset the timescale
        Time.timeScale = 1;

        // Enable player's Audio Sources
        foreach (AudioSource audioSource in playerAudioSources)
        {
            audioSource.enabled = true;
        }
    }

    public void OnMenuButtonPressed()
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
