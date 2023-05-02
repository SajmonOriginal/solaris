using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    // Reference na objekty pro orientaci a hráče
    public Transform orientation;

    public Transform playerObj;

    // Reference na Rigidbody a PlayerMovement
    private Rigidbody rb;

    private PlayerMovement pm;

    [Header("Sliding")]
    public float maxSlideTime; // Maximální doba klouzání

    public float slideForce; // Síla klouzání

    private float slideTimer; // Časovač klouzání

    public float slideYScale; // Y-ová škála objektu hráče během klouzání

    private float startYScale; // Původní Y-ová škála objektu hráče

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl; // Klávesa pro klouzání

    private float horizontalInput; // Horizontální vstup hráče

    private float verticalInput; // Vertikální vstup hráče

    // Metoda Start se volá při inicializaci objektu
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    // Metoda Update se volá každý snímek
    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Začátek klouzání, pokud je stisknuta klávesa pro klouzání a hráč se pohybuje
        if (
            Input.GetKeyDown(slideKey) &&
            (horizontalInput != 0 || verticalInput != 0)
        ) StartSlide();

        // Zastavení klouzání, pokud je klávesa pro klouzání uvolněna a hráč klouže
        if (Input.GetKeyUp(slideKey) && pm.sliding) StopSlide();
    }

    // Metoda FixedUpdate se volá každý pevný snímek
    private void FixedUpdate()
    {
        if (pm.sliding) SlidingMovement();
    }

    // Metoda pro začátek klouzání
    private void StartSlide()
    {
        pm.sliding = true;

        // Změna velikosti hráče
        playerObj.localScale =
            new Vector3(playerObj.localScale.x,
                slideYScale,
                playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    // Metoda pro pohyb při klouzání
    private void SlidingMovement()
    {
        Vector3 inputDirection =
            orientation.forward * verticalInput +
            orientation.right * horizontalInput;

        // Normální klouzání (ne po svahu)
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb
                .AddForce(inputDirection.normalized * slideForce,
                ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        else
        // Klouzání po svahu
        {
            rb
                .AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce,
                ForceMode.Force);
        }

        // Zastaví klouzání, pokud vyprší časovač
        if (slideTimer <= 0) StopSlide();
    }

    // Metoda pro zastavení klouzání
    private void StopSlide()
    {
        pm.sliding = false;

        // Obnoví velikost hráče
        playerObj.localScale =
            new Vector3(playerObj.localScale.x,
                startYScale,
                playerObj.localScale.z);
    }
}
