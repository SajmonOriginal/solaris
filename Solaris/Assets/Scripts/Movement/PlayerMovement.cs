using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    // Sekce pro nastavení rychlosti pohybu hráče
    [Header("Movement")]
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    public float climbSpeed;
    public float vaultSpeed;
    public float airMinSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    // Sekce pro nastavení skákání hráče
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    // Sekce pro nastavení dřepu hráče
    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    // Sekce pro nastavení klávesových zkratek
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    // Sekce pro nastavení detekce země
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    // Sekce pro nastavení manipulace se svahem
    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    // Sekce pro nastavení zvuků
    [Header("Audio")]
    public AudioSource walkingSound;
    public AudioSource jumpingSound;
    public AudioSource wallRunningSound;
    public AudioSource slidingSound;
    public AudioSource landingSound;
    public AudioSource windSound;

    // Sekce pro referenci na ostatní objekty a skripty
    [Header("References")]
    public Climbing climbingScript;
    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    // Vymezuje stavy pohybu hráče
    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        wallrunning,
        climbing,
        vaulting,
        crouching,
        sliding,
        air,
        jump
    }

    // Proměnné pro různé typy pohybu
    public bool sliding;
    public bool crouching;
    public bool wallrunning;
    public bool climbing;
    public bool vaulting;

    // Proměnné pro speciální režimy pohybu
    public bool freeze;
    public bool unlimited;
    
    public bool restricted;

    private float lastYVelocity;

    // Inicializace na začátku hry
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

    }

    // Aktualizace pohybu hráče, zvuků a jiných faktorů každý snímek
    private void Update()
    {
        // Detekce země
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        HandleSound();

        // Změna odporu vzduchu na zemi a ve vzduchu
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // Přehrání zvuku po dopadu na zem s vysokou rychlostí
        if (grounded && lastYVelocity < -10f)
        {
            if (!landingSound.isPlaying)
                landingSound.Play();
        }

        // Přehrání zvuku větru při pohybu hráče rychlostí sprintu ve vzduchu
        if (!grounded && rb.velocity.magnitude >= sprintSpeed)
        {
            if (!windSound.isPlaying)
                windSound.Play();
        }
        else
        {
            windSound.Stop();
        }

        lastYVelocity = rb.velocity.y;
    }

    // Aktualizace pohybu hráče každý fyzikální snímek
    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Přehrání zvuků pohybu hráče podle aktuálního stavu
    private void HandleSound()
    {
        // Kontrola, zda se hráč pohybuje
        bool isMoving = rb.velocity.magnitude > 0.1f;

        switch (state)
        {
            case MovementState.walking:
            case MovementState.sprinting:
                if (isMoving)
                {
                    if (!walkingSound.isPlaying)
                        walkingSound.Play();
                }
                else
                {
                    walkingSound.Stop();
                }
                break;
            case MovementState.jump:
                if (!jumpingSound.isPlaying && isMoving)
                {
                    jumpingSound.Play();
                }
                break;
            case MovementState.wallrunning:
                if (isMoving)
                {
                    if (!wallRunningSound.isPlaying)
                        wallRunningSound.Play();
                }
                else
                {
                    wallRunningSound.Stop();
                }
                break;
            case MovementState.sliding:
                if (isMoving)
                {
                    if (!slidingSound.isPlaying)
                        slidingSound.Play();
                }
                else
                {
                    slidingSound.Stop();
                }
                break;
            default:
                // Zastavení všech zvuků
                StopAllSounds();
                break;
        }
    }

    // Zastavení všech zvuků
    private void StopAllSounds()
    {
        walkingSound.Stop();
        jumpingSound.Stop();
        wallRunningSound.Stop();
        slidingSound.Stop();
    }

    // Zpracování vstupu hráče
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Skákání
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            state = MovementState.jump;
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Začátek dřepu
        if (Input.GetKeyDown(crouchKey) && horizontalInput == 0 && verticalInput == 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            crouching = true;
        }

        // Konec dřepu
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

            crouching = false;
        }
    }

    bool keepMomentum;
    // Zpracování aktuálního stavu pohybu hráče
    private void StateHandler()
    {
        // Režim - Zamrznutí
        if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;
        }

        // Režim - Neomezený pohyb
        else if (unlimited)
        {
            state = MovementState.unlimited;
            desiredMoveSpeed = 999f;
        }

        // Režim - Skok přes překážku
        else if (vaulting)
        {
            state = MovementState.vaulting;
            desiredMoveSpeed = vaultSpeed;
        }

        // Režim - Lezení
        else if (climbing)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
        }

        // Režim - Běh po zdi
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }

        // Režim - Klouzání
        else if (sliding)
        {
            state = MovementState.sliding;

            // Zvyšování rychlosti klouzání každou sekundu
            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
                keepMomentum = true;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }

        // Režim - Dřep
        else if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        // Režim - Sprint
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        // Režim - Chůze
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Režim - Vzduch
        else
        {
            state = MovementState.air;

            if (moveSpeed < airMinSpeed)
                desiredMoveSpeed = airMinSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

        // Deaktivace udržování momentu
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    // Plynulé přechodné zvyšování rychlosti pohybu
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    // Získání směru pohybu hráče
    public Vector3 GetMoveDirection()
    {
        Vector3 moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.Normalize();
        return moveDirection;
    }

    // Pohyb hráče
    private void MovePlayer()
    {
        if (climbingScript.exitingWall) return;
        if (restricted) return;

        // Výpočet směru pohybu
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Na svahu
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // Na zemi
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // Ve vzduchu
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // Vypnutí gravitace při běhu po zdi
        if(!wallrunning) rb.useGravity = !OnSlope();
    }

    // Omezení rychlosti pohybu hráče
    private void SpeedControl()
    {
        // Omezení rychlosti na svahu
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // Omezení rychlosti na zemi nebo ve vzduchu
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    // Skok hráče
    private void Jump()
    {
        exitingSlope = true;

        // Resetování rychlosti ve vertikálním směru
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Přidání síly ve směru nahoru
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // Resetování možnosti skoku
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    // Kontrola, zda se hráč nachází na svahu
    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    // Získání směru pohybu na svahu
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    // Zaokrouhlení čísla na zadaný počet desetinných míst
    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }
}
