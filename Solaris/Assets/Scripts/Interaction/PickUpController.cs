using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public bool isEquipped;
    public bool slotFull;

    public Transform attackPoint; // Nově přidané

    private Rigidbody rb;
    private BoxCollider coll;
    private Quaternion desiredRotation;
    private Vector3 desiredPosition;
    private Vector3 posVelocity;
    private float smoothSpeed = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponentInChildren<BoxCollider>();

        if (!isEquipped)
        {
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        else
        {
            rb.isKinematic = true;
            coll.isTrigger = true;
            ToggleWeaponRenderer(false); // Vypnout vykreslování zbraně, pokud je nesebrána
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float distanceToPlayer = Vector3.Distance(transform.position, GameObject.Find("Player").transform.position);

            if (distanceToPlayer < 2f && !slotFull)
            {
                PickUp();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && isEquipped)
        {
            Drop();
        }
    }

    private void PickUp()
    {
        slotFull = true;
        rb.isKinematic = true;
        coll.isTrigger = true;
        transform.SetParent(GameObject.Find("GunPos").transform);
        transform.localPosition = Vector3.zero;
        desiredPosition = Vector3.zero;
        desiredRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        ProjectileGun projectileGun = GetComponent<ProjectileGun>();
        if (projectileGun != null)
        {
            projectileGun.SetEquipped(true);
        }

        ToggleWeaponRenderer(true); // Zapnout vykreslování zbraně, pokud je sebrána
        UpdateCameraCullingMask(false); // Vypnout vykreslování whatIsWeapon ve GunCam

        if (attackPoint != null) // Nově přidané
        {
            desiredRotation = Quaternion.LookRotation(attackPoint.position - transform.position);
        }
    }

    private void Drop()
    {
        slotFull = false;
        rb.isKinematic = false;
        coll.isTrigger = false;
        transform.SetParent(null);

        ProjectileGun projectileGun = GetComponent<ProjectileGun>();
        if (projectileGun != null)
        {
            projectileGun.SetEquipped(false);
        }

        ToggleWeaponRenderer(false); // Vypnout vykreslování zbraně
        UpdateCameraCullingMask(true); // Zapnout vykreslování whatIsWeapon ve GunCam
    }

    private void ToggleWeaponRenderer(bool isEnabled)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            if (renderer.gameObject.layer == LayerMask.NameToLayer("whatIsWeapon"))
            {
                renderer.enabled = isEnabled;
            }
        }
    }

    private void UpdateCameraCullingMask(bool enableWhatIsWeapon)
    {
        Camera playerCam = GameObject.Find("PlayerCam").GetComponent<Camera>();
        Camera gunCam = GameObject.Find("GunCam").GetComponent<Camera>();

        if (playerCam != null && gunCam != null)
        {
            playerCam.cullingMask = enableWhatIsWeapon ? playerCam.cullingMask | (1 << LayerMask.NameToLayer("whatIsWeapon")) : playerCam.cullingMask & ~(1 << LayerMask.NameToLayer("whatIsWeapon"));
            gunCam.cullingMask = enableWhatIsWeapon ? gunCam.cullingMask & ~(1 << LayerMask.NameToLayer("whatIsWeapon")) : gunCam.cullingMask | (1 << LayerMask.NameToLayer("whatIsWeapon"));
        }
    }

    private void FixedUpdate()
    {
        if (isEquipped)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, desiredRotation, Time.fixedDeltaTime * smoothSpeed);
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, desiredPosition, ref posVelocity, 1f / smoothSpeed);
        }
    }
}
