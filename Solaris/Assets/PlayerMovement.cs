using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed; // Aktuální rychlost pohybu hráče

    public float walkSpeed; // Rychlost chůze

    public float sprintSpeed; // Rychlost sprintu

    public float slideSpeed; // Rychlost skluzu

    private float desiredMoveSpeed; // Požadovaná rychlost pohybu hráče

    private float lastDesiredMoveSpeed; // Poslední požadovaná rychlost pohybu hráče

    public float speedIncreaseMultiplier; // Násobitel zvýšení rychlosti

    public float slopeIncreaseMultiplier; // Násobitel zvýšení rychlosti na svahu

    public float groundDrag; // Tření při pohybu po zemi

    [Header("Jumping")]
    public float jumpForce; // Síla skoku

    public float jumpCooldown; // Prodleva mezi skoky

    public float airMultiplier; // Násobitel pohybu ve vzduchu

    bool readyToJump; // Připravenost ke skoku

    [Header("Crouching")]
    public float crouchSpeed; // Rychlost při dřepu

    public float crouchYScale; // Měřítko hráče při dřepu

    private float startYScale; // Původní měřítko hráče

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space; // Klávesa pro skok

    public KeyCode sprintKey = KeyCode.LeftShift; // Klávesa pro sprint

    public KeyCode crouchKey = KeyCode.LeftControl; // Klávesa pro dřep

    [Header("Ground Check")]
    public float playerHeight; // Výška hráče

    public LayerMask whatIsGround; // Co je považováno za zemi

    bool grounded; // Je hráč na zemi?

    [Header("Slope Handling")]
    public float maxSlopeAngle; // Maximální úhel svahu, na kterém může hráč stát

    private RaycastHit slopeHit; // Informace o objektu svahu

    private bool exitingSlope; // Opouštění svahu

    public Transform orientation; // Směr pohybu hráče

    float horizontalInput; // Horizontální vstup

    float verticalInput; // Vertikální vstup

    Vector3 moveDirection; // Směr pohybu hráče

    Rigidbody rb; // Rigidbody hráče

    public MovementState state; // Stav pohybu hráče

    public enum
    MovementState // Výčet stavů pohybu hráče
    {
        walking,
        sprinting,
        crouching,
        sliding,
        air
    }

    public bool sliding; // Je hráč ve skluzu?

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Získání Rigidbody hráče
        rb.freezeRotation = true; // Zamrznutí rotace hráče

        readyToJump = true; // Hráč je připraven skočit

        startYScale = transform.localScale.y; // Uložení původního měřítka hráče
    }

    private void Update()
    {
        // Kontrola země
        grounded =
            Physics
                .Raycast(transform.position,
                Vector3.down,
                playerHeight * 0.5f + 0.2f,
                whatIsGround);

        MyInput(); // Zpracování vstupů
        SpeedControl(); // Řízení rychlosti
        StateHandler(); // Řízení stavu pohybu hráče

        // Nastavení tření
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer(); // Pohyb hráče
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // Získání horizontálního vstupu
        verticalInput = Input.GetAxisRaw("Vertical"); // Získání vertikálního vstupu

        // Kdy skákat
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump(); // Skok

            Invoke(nameof(ResetJump), jumpCooldown); // Resetování skoku
        }

        // Začátek dřepu
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale =
                new Vector3(transform.localScale.x,
                    crouchYScale,
                    transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // Konec dřepu
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale =
                new Vector3(transform.localScale.x,
                    startYScale,
                    transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        // Mód - Skluz
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        } // Mód - Dřep
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        } // Mód - Sprint
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        } // Mód - Chůze
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        // Mód - Ve vzduchu
        {
            state = MovementState.air;
        }

        // Kontrola, zda se desiredMoveSpeed výrazně změnila
        if (
            Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f &&
            moveSpeed != 0
        )
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed()); // Plynulé změny rychlosti
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // Plynulé přechody mezi rychlostmi pohybu
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed =
                Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time +=
                    Time.deltaTime *
                    speedIncreaseMultiplier *
                    slopeIncreaseMultiplier *
                    slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        // Výpočet směru pohybu
        moveDirection =
            orientation.forward * verticalInput +
            orientation.right * horizontalInput;

        // Na svahu
        if (OnSlope() && !exitingSlope)
        {
            rb
                .AddForce(GetSlopeMoveDirection(moveDirection) *
                moveSpeed *
                20f,
                ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else // Na zemi
        if (grounded)
            rb
                .AddForce(moveDirection.normalized * moveSpeed * 10f,
                ForceMode.Force);
        else // Ve vzduchu
        if (!grounded)
            rb
                .AddForce(moveDirection.normalized *
                moveSpeed *
                10f *
                airMultiplier,
                ForceMode.Force);

        // Vypnutí gravitace na svahu
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // Omezení rychlosti na svahu
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        // Omezení rychlosti na zemi nebo ve vzduchu
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Omezení rychlosti, pokud je to nutné
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity =
                    new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // Resetování rychlosti ve směru osy Y
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); // Přidání síly pro skok
    }

    private void ResetJump()
    {
        readyToJump = true; // Hráč je připraven skočit

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (
            Physics
                .Raycast(transform.position,
                Vector3.down,
                out slopeHit,
                playerHeight * 0.5f + 0.3f)
        )
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        // Výpočet směru pohybu na svahu
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
