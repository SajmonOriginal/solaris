using UnityEngine;

public class TargetObject : MonoBehaviour
{
    public WinMenu winMenu; // Přetáhněte skript "WinMenu" sem

    private bool hasWon = false;
    private Vector3 initialPosition; // Počáteční pozice poháru

    public float floatAmplitude = 1f; // Amplituda pohybu levitace
    public float floatSpeed = 1f; // Rychlost pohybu levitace
    public float rotationSpeed = 1f; // Rychlost otáčení poháru

    private void Start()
    {
        initialPosition = transform.position; // Uložit počáteční pozici poháru
    }

    private void Update()
    {
  
            // Otáčení poháru
            float z = Mathf.PingPong(Time.time, 1f);
            Vector3 axis = new Vector3(1f, 1f, z);
            base.transform.Rotate(axis, rotationSpeed);

            // Levitace poháru ve vzduchu
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = initialPosition + new Vector3(0f, yOffset, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasWon && other.CompareTag("Player"))
        {
            // Zobrazit menu vítězství
            winMenu.ShowHide(true);

            // Zastavit hru
            Time.timeScale = 0;

            hasWon = true;

            // Skrytí kurzoru myši
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
