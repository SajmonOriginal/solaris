using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public DeathMenu deathMenu; // Reference na DeathMenu skript pro zobrazení tlačítka Retry

    public GameObject crosshair; // Reference na objekt Crosshair pro skrytí

    // Funkce volaná při zranění hráče
    public void TakeDamage(int damage)
    {
        Die(); // Zavolá funkci pro smrt hráče
    }

    // Funkce pro smrt hráče
    public void Die()
    {
        // Zobrazení tlačítka Retry
        deathMenu.ShowHide(true);

        // Zmrazení hry
        Time.timeScale = 0;

        // Zobrazení kurzoru
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Skrytí Crosshairu
        crosshair.SetActive(false);
    }
}
