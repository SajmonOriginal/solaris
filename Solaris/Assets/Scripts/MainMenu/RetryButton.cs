using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    public GameObject mainMenuButton; // Drag the MainMenu button UI here
    public GameObject retryButton; // Drag the Retry button UI here
    public GameObject crosshair; // Drag your Crosshair object here

    private void Start()
    {
        // Hide the Retry and MainMenu buttons at the start of the game
        ShowHide(false);
    }

    public void ShowHide(bool show)
    {
        retryButton.SetActive(show);
        mainMenuButton.SetActive(show); // This line will hide/show the MainMenu button with the Retry button
        crosshair.SetActive(!show); // this line will hide/show the crosshair inversely
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // toggle the display of the Retry and MainMenu buttons
            ShowHide(!retryButton.activeSelf);

            // freeze or unfreeze the game
            Time.timeScale = retryButton.activeSelf ? 0 : 1;

            // show or hide the cursor
            Cursor.lockState = retryButton.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = retryButton.activeSelf;
        }
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
    }

    public void OnMainMenuButtonPressed()
    {
        // Load the main menu
        SceneManager.LoadScene("Main Menu");

        // Reset the timescale
        Time.timeScale = 1;
    }
}
