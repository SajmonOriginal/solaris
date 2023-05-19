using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public ProjectileGun gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;
    public Camera PlayerCam;
    public Camera GunCam;

    public float pickUpRange, pickUpTime;
    public float dropForwardForce, dropUpwardForce;

    public bool isEquipped;
    public static bool slotFull;

    private int whatIsWeaponLayer;

    private void Start()
    {
        if (!isEquipped)
        {
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
            transform.SetParent(null); // if the weapon is not equipped, we make sure its parent is null
        }
        if (isEquipped)
        {
            slotFull = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            transform.SetParent(gunContainer); // if the weapon is equipped, we make sure its parent is gunContainer
        }

        whatIsWeaponLayer = LayerMask.NameToLayer("whatIsWeapon");
        SetCullingMasks(isEquipped);
    }

    private void Update()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!isEquipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();

        if (isEquipped && Input.GetKeyDown(KeyCode.Q)) Drop();
    }

    private void PickUp()
    {
        isEquipped = true;
        slotFull = true;

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        rb.isKinematic = true;
        coll.isTrigger = true;

        gunScript.enabled = true;

        SetCullingMasks(true);
    }

    private void Drop()
    {
        isEquipped = false;
        slotFull = false;

        transform.SetParent(null);

        rb.isKinematic = false;
        coll.isTrigger = false;

        rb.velocity = player.GetComponentInParent<Rigidbody>().velocity;

        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

        gunScript.enabled = false;

        SetCullingMasks(false);
    }

    private void SetCullingMasks(bool isEquipped)
    {
        if (isEquipped)
        {
            PlayerCam.cullingMask &= ~(1 << whatIsWeaponLayer);
            GunCam.cullingMask |= (1 << whatIsWeaponLayer);
        }
        else
        {
            PlayerCam.cullingMask |= (1 << whatIsWeaponLayer);
            GunCam.cullingMask &= ~(1 << whatIsWeaponLayer);
        }
    }
}
