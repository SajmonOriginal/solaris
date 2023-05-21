using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall; // Vrstva, která reprezentuje zeď

    public LayerMask whatIsGround; // Vrstva, která reprezentuje zem

    public float wallRunForce; // Síla, která se aplikuje na hráče při wallrunu

    public float wallJumpUpForce; // Síla, která se aplikuje na hráče při wall jumpu směrem nahoru

    public float wallJumpSideForce; // Síla, která se aplikuje na hráče při wall jumpu do stran

    public float wallClimbSpeed; // Rychlost, kterou hráč dosáhne při pohybu nahoru nebo dolů po zdi

    public float maxWallRunTime; // Maximální doba, po kterou hráč může wallrun provádět

    private float wallRunTimer; // Časovač pro wallrun

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space; // Klávesa pro skok

    public KeyCode upwardsRunKey = KeyCode.LeftShift; // Klávesa pro pohyb nahoru po zdi

    public KeyCode downwardsRunKey = KeyCode.LeftControl; // Klávesa pro pohyb dolů po zdi

    private bool upwardsRunning; // Příznak, zda hráč provádí pohyb nahoru po zdi

    private bool downwardsRunning; // Příznak, zda hráč provádí pohyb dolů po zdi

    private float horizontalInput; // Vstup z klávesnice pro horizontální pohyb

    private float verticalInput; // Vstup z klávesnice pro vertikální pohyb

    [Header("Detection")]
    public float wallCheckDistance; // Vzdálenost, ve které se detekuje zeď

    public float minJumpHeight; // Minimální výška, kterou hráč musí dosáhnout, aby se nepovažoval za na zemi

    private RaycastHit leftWallhit; // Informace o levé zdi po detekci

    private RaycastHit rightWallhit; // Informace o pravé zdi po detekci

    private bool wallLeft; // Příznak, zda je detekována levá zeď

    private bool wallRight; // Příznak, zda je detekována pravá zeď

    [Header("Exiting")]
    private bool exitingWall; // Příznak, zda hráč opouští wallrun

    public float exitWallTime; // Doba, po kterou hráč zůstává ve stavu opouštění wallrunu

    private float exitWallTimer; // Časovač pro opouštění wallrunu

    [Header("Gravity")]
    public bool useGravity; // Příznak, zda se používá gravitace

    public float gravityCounterForce; // Síla, která vyvažuje gravitační sílu

    [Header("References")]
    public Transform orientation; // Odkaz na transformaci hráče

    public PlayerCam cam; // Odkaz na kameru hráče

    private PlayerMovement pm; // Odkaz na komponentu PlayerMovement

    private LedgeGrabbing lg; // Odkaz na komponentu LedgeGrabbing

    private Rigidbody rb; // Odkaz na komponentu Rigidbody

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        lg = GetComponent<LedgeGrabbing>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning) WallRunningMovement();
    }

    private void CheckForWall()
    {
        // Detekce zdi na levé straně hráče
        wallLeft =
            Physics
                .Raycast(transform.position,
                -orientation.right,
                out leftWallhit,
                wallCheckDistance,
                whatIsWall);

        // Detekce zdi na pravé straně hráče
        wallRight =
            Physics
                .Raycast(transform.position,
                orientation.right,
                out rightWallhit,
                wallCheckDistance,
                whatIsWall);
    }

    private bool AboveGround()
    {
        // Zjištění, zda se hráč nachází nad zemí
        return !Physics
            .Raycast(transform.position,
            Vector3.down,
            minJumpHeight,
            whatIsGround);
    }

    private void StateMachine()
    {
        // Získání vstupů z klávesnice
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // Stav 1 - Wallrunning
        if (
            (wallLeft || wallRight) &&
            verticalInput > 0 &&
            AboveGround() &&
            !exitingWall
        )
        {
            if (!pm.wallrunning) StartWallRun();

            // Časovač wallrunu
            if (wallRunTimer > 0) wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            // Wall jump
            if (Input.GetKeyDown(jumpKey)) WallJump();
        }
        else // Stav 2 - Opouštění wallrunu
        if (exitingWall)
        {
            if (pm.wallrunning) StopWallRun();

            if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0) exitingWall = false;
        }
        else
        // Stav 3 - Žádný
        {
            if (pm.wallrunning) StopWallRun();
        }
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Aplikace efektů kamery
        cam.DoFov(90f);
        if (wallLeft) cam.DoTilt(-5f);
        if (wallRight) cam.DoTilt(5f);
    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal =
            wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if (
            (orientation.forward - wallForward).magnitude >
            (orientation.forward - -wallForward).magnitude
        ) wallForward = -wallForward;

        // Síla pohybu vpřed
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // Síla pohybu nahoru/dolů
        if (upwardsRunning)
            rb.velocity =
                new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        if (downwardsRunning)
            rb.velocity =
                new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

        // Síla tlačící k zdi
        if (
            !(wallLeft && horizontalInput > 0) &&
            !(wallRight && horizontalInput < 0)
        ) rb.AddForce(-wallNormal * 100, ForceMode.Force);

        // Oslabení gravitace
        if (useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;

        // Obnovení efektů kamery
        cam.DoFov(80f);
        cam.DoTilt(0f);
    }

    private void WallJump()
    {
        if (lg.holding || lg.exitingLedge) return;

        // Vstup do stavu opouštění wallrunu
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal =
            wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 forceToApply =
            transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // Obnovení y rychlosti a aplikace síly
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
