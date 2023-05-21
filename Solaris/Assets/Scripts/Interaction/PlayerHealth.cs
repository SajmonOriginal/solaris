using UnityEngine;
using UnityEngine.UI; // Přidejte tuto řádku na začátku skriptu

public class PlayerHealth : MonoBehaviour
{
    public DeathMenu deathMenu;
    public GameObject crosshair; // Přidejte tuto řádku. Ve vašem Unity Editoru přetáhněte objekt Crosshair do tohoto pole v inspektor

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        // zobrazit tlačítko Retry
        deathMenu.ShowHide(true);

        // zmrazit hru
        Time.timeScale = 0;

        // zobrazit kurzor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // skryjeme Crosshair
        crosshair.SetActive(false);
    }
}
