using UnityEngine;

public class TargetObject : MonoBehaviour
{
    public WinMenu winMenu; // Odkaz na skript "WinMenu", který je přetažen sem

    private bool hasWon = false;

    private Vector3 initialPosition; // Počáteční pozice poháru

    public float floatAmplitude = 1f; // Amplituda pohybu při levitaci

    public float floatSpeed = 1f; // Rychlost pohybu při levitaci

    public float rotationSpeed = 1f; // Rychlost otáčení poháru

    private void Start()
    {
        initialPosition = transform.position; // Uložení počáteční pozice poháru
    }

    private void Update()
    {
        // Otáčení poháru
        float z = Mathf.PingPong(Time.time, 1f); // Výpočet hodnoty z pro otáčení poháru
        Vector3 axis = new Vector3(1f, 1f, z); // Vytvoření osy pro otáčení poháru
        transform.Rotate (axis, rotationSpeed); // Otáčení poháru kolem osy s danou rychlostí

        // Levitace poháru ve vzduchu
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude; // Výpočet hodnoty yOffset pro levitaci poháru
        transform.position = initialPosition + new Vector3(0f, yOffset, 0f); // Přesunutí poháru na novou pozici s daným offsetem ve směru osy y
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasWon && other.CompareTag("Player"))
        {
            // Zobrazení menu vítězství
            winMenu.ShowHide(true);

            // Zastavení hry
            Time.timeScale = 0;

            hasWon = true;

            // Skrytí kurzoru myši
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
