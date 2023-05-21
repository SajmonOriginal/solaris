using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabbing : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement pm; // Odkaz na PlayerMovement
    public Transform orientation; // Odkaz na transformaci orientace
    public Transform cam; // Odkaz na transformaci kamery
    public Rigidbody rb; // Odkaz na komponentu Rigidbody

    [Header("Ledge Grabbing")]
    public float moveToLedgeSpeed; // Rychlost pohybu k okraji
    public float maxLedgeGrabDistance; // Maximální vzdálenost pro chytání okraje

    public float minTimeOnLedge; // Minimální doba na okraji
    private float timeOnLedge; // Aktuální doba na okraji

    public bool holding; // Příznak, zda je hráč na okraji

    [Header("Ledge Jumping")]
    public KeyCode jumpKey = KeyCode.Space; // Klávesa pro skok
    public float ledgeJumpForwardForce; // Síla skoku do předu z okraje
    public float ledgeJumpUpwardForce; // Síla skoku nahoru z okraje

    [Header("Ledge Detection")]
    public float ledgeDetectionLength; // Délka detekce okraje
    public float ledgeSphereCastRadius; // Poloměr pro SphereCast detekci okraje
    public LayerMask whatIsLedge; // Vrstva určující, co je považováno za okraj

    private Transform lastLedge; // Poslední detekovaný okraj
    private Transform currLedge; // Aktuálně držený okraj

    private RaycastHit ledgeHit; // Informace o zásahu okraje

    [Header("Exiting")]
    public bool exitingLedge; // Příznak, zda hráč opouští okraj
    public float exitLedgeTime; // Doba trvání opouštění okraje
    private float exitLedgeTimer; // Aktuální doba opouštění okraje

    private void Update()
    {
        LedgeDetection(); // Detekce okraje
        SubStateMachine(); // Podstavový stav
    }

    private void SubStateMachine()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool anyInputKeyPressed = horizontalInput != 0 || verticalInput != 0;

        // Podstav 1 - Držení okraje
        if (holding)
        {
            FreezeRigidbodyOnLedge();

            timeOnLedge += Time.deltaTime;

            if (timeOnLedge > minTimeOnLedge && anyInputKeyPressed) ExitLedgeHold();

            if (Input.GetKeyDown(jumpKey)) LedgeJump();
        }

        // Podstav 2 - Opouštění okraje
        else if (exitingLedge)
        {
            if (exitLedgeTimer > 0) exitLedgeTimer -= Time.deltaTime;
            else exitingLedge = false;
        }
    }

    private void LedgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDetectionLength, whatIsLedge);

        if (!ledgeDetected) return;

        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);

        if (ledgeHit.transform == lastLedge) return;

        if (distanceToLedge < maxLedgeGrabDistance && !holding) EnterLedgeHold();
    }

    private void LedgeJump()
    {
        ExitLedgeHold();

        Invoke(nameof(DelayedJumpForce), 0.05f);
    }

    private void DelayedJumpForce()
    {
        Vector3 forceToAdd = cam.forward * ledgeJumpForwardForce + orientation.up * ledgeJumpUpwardForce;
        rb.velocity = Vector3.zero;
        rb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    private void EnterLedgeHold()
    {
        holding = true;

        pm.unlimited = true;
        pm.restricted = true;

        currLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    private void FreezeRigidbodyOnLedge()
    {
        rb.useGravity = false;

        Vector3 directionToLedge = currLedge.position - transform.position;
        float distanceToLedge = Vector3.Distance(transform.position, currLedge.position);

        // Pohyb hráče směrem k okraji
        if (distanceToLedge > 1f)
        {
            if (rb.velocity.magnitude < moveToLedgeSpeed)
                rb.AddForce(directionToLedge.normalized * moveToLedgeSpeed * 1000f * Time.deltaTime);
        }

        // Držení se okraje
        else
        {
            if (!pm.freeze) pm.freeze = true;
            if (pm.unlimited) pm.unlimited = false;
        }

        // Opouštění okraje, pokud se něco pokazí
        if (distanceToLedge > maxLedgeGrabDistance) ExitLedgeHold();
    }

    private void ExitLedgeHold()
    {
        exitingLedge = true;
        exitLedgeTimer = exitLedgeTime;

        holding = false;
        timeOnLedge = 0f;

        pm.restricted = false;
        pm.freeze = false;

        rb.useGravity = true;

        StopAllCoroutines();
        Invoke(nameof(ResetLastLedge), 1f);
    }

    private void ResetLastLedge()
    {
        lastLedge = null;
    }
}
