using UnityEngine;

public class Lava : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Zkontrolujeme, jestli došlo ke kolizi s objektem označeným jako "Player"
        if (collision.collider.CompareTag("Player"))
        {
            // Získáme referenci na komponentu PlayerHealth připojenou ke koliznímu objektu
            PlayerHealth playerHealth =
                collision.collider.GetComponent<PlayerHealth>();

            // Pokud byl nalezen objekt s komponentou PlayerHealth
            if (playerHealth != null)
            {
                // Zavoláme metodu Die() na objektu PlayerHealth, která zpracuje smrt hráče
                playerHealth.Die();
            }
        }
    }
}
