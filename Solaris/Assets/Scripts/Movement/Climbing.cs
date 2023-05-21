using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation; // Odkaz na transformaci orientace
    public Rigidbody rb; // Odkaz na komponentu Rigidbody
    public PlayerMovement pm; // Odkaz na skript PlayerMovement
    public LedgeGrabbing lg; // Odkaz na skript LedgeGrabbing
    public LayerMask whatIsWall; // Vrstva určující, co je považováno za stěnu

    [Header("Climbing")]
    public float climbSpeed; // Rychlost lezení
    public float maxClimbTime; // Maximální doba lezení
    private float climbTimer; // Aktuální doba lezení

    private bool climbing; // Příznak, zda hráč leze

    [Header("Climb Jumping")]
    public float climbJumpUpForce; // Síla odrazu nahoru při skoku z lezení
    public float climbJumpBackForce; // Síla odrazu dozadu při skoku z lezení

    public KeyCode jumpKey = KeyCode.Space; // Klávesa pro skok
    public int climbJumps; // Počet skoků z lezení
    private int climbJumpsLeft; // Počet zbývajících skoků z lezení

    [Header("Detection")]
    public float detectionLength; // Délka detekce stěny
    public float sphereCastRadius; // Poloměr pro SphereCast detekci
    public float maxWallLookAngle; // Maximální úhel, ve kterém se hráč dívá na stěnu
    private float wallLookAngle; // Aktuální úhel, ve kterém se hráč dívá na stěnu

    private RaycastHit frontWallHit; // Informace o zásahu stěny vpředu
    private bool wallFront; // Příznak, zda je stěna před hráčem

    private Transform lastWall; // Poslední detekovaná stěna
    private Vector3 lastWallNormal; // Normála poslední detekované stěny
    public float minWallNormalAngleChange; // Minimální změna úhlu normály stěny, aby byla považována za novou stěnu

    [Header("Exiting")]
    public bool exitingWall; // Příznak, zda hráč opouští stěnu
    public float exitWallTime; // Doba trvání opouštění stěny
    private float exitWallTimer; // Aktuální doba opouštění stěny

    private void Start()
    {
        lg = GetComponent<LedgeGrabbing>(); // Získání odkazu na skript LedgeGrabbing
    }

    private void Update()
    {
        WallCheck(); // Kontrola stěny
        StateMachine(); // Hlavní stavový automat

        if (climbing && !exitingWall) ClimbingMovement(); // Pohyb při lezení
    }

    private void StateMachine()
    {
        // Stav 0 - Držení okraje
        if (lg.holding)
        {
            if (climbing) StopClimbing(); // Přestání lezení

        }
        
        // Stav 1 - Lezení
        else if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if (!climbing && climbTimer > 0) StartClimbing(); // Začátek lezení

            // Časovač
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing(); // Přestání lezení
        }

        // Stav 2 - Opouštění
        else if (exitingWall)
        {
            if (climbing) StopClimbing(); // Přestání lezení

            if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
            if (exitWallTimer < 0) exitingWall = false; // Konec opouštění stěny
        }

        // Stav 3 - Žádný
        else
        {
            if (climbing) StopClimbing(); // Přestání lezení
        }

        if (wallFront && Input.GetKeyDown(jumpKey) && climbJumpsLeft > 0) ClimbJump(); // Skok z lezení
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall); // Kontrola přítomnosti stěny před hráčem
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal); // Výpočet úhlu, ve kterém se hráč dívá na stěnu

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange; // Příznak, zda je detekována nová stěna

        if ((wallFront && newWall) || pm.grounded)
        {
            climbTimer = maxClimbTime; // Resetování časovače lezení
            climbJumpsLeft = climbJumps; // Resetování počtu skoků z lezení
        }
    }

    private void StartClimbing()
    {
        climbing = true; // Nastavení stavu lezení
        pm.climbing = true; // Nastavení stavu lezení ve skriptu PlayerMovement

        lastWall = frontWallHit.transform; // Uložení poslední detekované stěny
        lastWallNormal = frontWallHit.normal; // Uložení normály poslední detekované stěny

        // Nápad - změna zorného pole kamery
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z); // Nastavení rychlosti lezení

    }

    private void StopClimbing()
    {
        climbing = false; // Vypnutí stavu lezení
        pm.climbing = false; // Vypnutí stavu lezení ve skriptu PlayerMovement

    }

    private void ClimbJump()
    {
        if (pm.grounded) return;
        if (lg.holding || lg.exitingLedge) return;

        print("climbjump");

        exitingWall = true; // Nastavení stavu opouštění stěny
        exitWallTimer = exitWallTime; // Nastavení časovače opouštění stěny

        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce; // Síla, která se aplikuje při skoku z lezení

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse); // Aplikace síly

        climbJumpsLeft--;
    }
}
