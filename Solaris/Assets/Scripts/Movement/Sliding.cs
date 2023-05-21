using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation; // Odkaz na transformaci orientace
    public Transform playerObj; // Odkaz na transformaci hráče
    private Rigidbody rb; // Odkaz na komponentu Rigidbody
    private PlayerMovement pm; // Odkaz na skript PlayerMovement

    [Header("Sliding")]
    public float maxSlideTime; // Maximální doba skluzu
    public float slideForce; // Síla skluzu
    private float slideTimer; // Aktuální doba skluzu

    public float slideYScale; // Výška hráče během skluzu
    private float startYScale; // Počáteční výška hráče

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl; // Klávesa pro skluz
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Získání komponenty Rigidbody
        pm = GetComponent<PlayerMovement>(); // Získání skriptu PlayerMovement

        startYScale = playerObj.localScale.y; // Uložení počáteční výšky hráče
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
            StartSlide(); // Zahájení skluzu

        if (Input.GetKeyUp(slideKey) && pm.sliding)
            StopSlide(); // Zastavení skluzu
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement(); // Pohyb při skluzu
    }

    private void StartSlide()
    {
        if (pm.wallrunning) return; // Pokud se hráč nachází na stěně, nepovol skluz

        pm.sliding = true; // Nastavení příznaku skluzu

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z); // Změna výšky hráče
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // Aplikace síly dolů

        slideTimer = maxSlideTime; // Nastavení doby skluzu
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Skluz po normále
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force); // Aplikace skluzové síly

            slideTimer -= Time.deltaTime; // Snížení času skluzu
        }
        // Skluz dolů po svahu
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force); // Aplikace skluzové síly po svahu
        }

        if (slideTimer <= 0)
            StopSlide(); // Zastavení skluzu, pokud vypršel čas skluzu
    }

    private void StopSlide()
    {
        pm.sliding = false; // Vypnutí příznaku skluzu

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z); // Vrácení výšky hráče na původní hodnotu
    }
}
