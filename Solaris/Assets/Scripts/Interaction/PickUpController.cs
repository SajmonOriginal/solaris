using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public ProjectileGun gunScript;

    public Rigidbody rb;

    public BoxCollider coll;

    public Transform

            player,
            gunContainer,
            fpsCam;

    public Camera PlayerCam;

    public Camera GunCam;

    public float pickUpRange; // Vzdálenost, ve které může hráč sebrat zbraň

    public float pickUpTime; // Čas, po který musí hráč držet tlačítko E pro sebrání zbraně

    public float dropForwardForce; // Síla, která je aplikována při hození zbraně vpřed

    public float dropUpwardForce; // Síla, která je aplikována při hození zbraně nahoru

    public bool isEquipped; // Určuje, zda je zbraň vybavena hráčem

    public static bool slotFull; // Určuje, zda je slot pro zbraň plný

    private int whatIsWeaponLayer; // Číslo vrstvy, která reprezentuje zbraň

    private void Start()
    {
        // Inicializace stavu zbraně při spuštění
        if (!isEquipped)
        {
            gunScript.enabled = false; // Zbraň je vypnutá
            rb.isKinematic = false; // Nekinematické pohyby jsou povoleny
            coll.isTrigger = false; // Kolidování je povoleno
            transform.SetParent(null); // Pokud zbraň není vybavena, zajistíme, že nemá žádného rodiče
        }
        if (isEquipped)
        {
            slotFull = true; // Slot pro zbraň je plný
            rb.isKinematic = true; // Kinematické pohyby jsou povoleny
            coll.isTrigger = true; // Kolidování je vypnuto
            transform.SetParent (gunContainer); // Pokud je zbraň vybavena, zajistíme, že jejím rodičem je gunContainer
        }

        whatIsWeaponLayer = LayerMask.NameToLayer("whatIsWeapon"); // Získání čísla vrstvy "whatIsWeapon"
        SetCullingMasks (isEquipped); // Nastavení masky pro vykreslování kamer
    }

    private void Update()
    {
        // Zjištění vzdálenosti mezi hráčem a zbraní
        Vector3 distanceToPlayer = player.position - transform.position;

        // Pokud zbraň není vybavena, hráč je v dosahu zbraně, stiskne klávesu E a slot pro zbraň není plný, provedeme sebrání zbraně
        if (
            !isEquipped &&
            distanceToPlayer.magnitude <= pickUpRange &&
            Input.GetKeyDown(KeyCode.E) &&
            !slotFull
        )
        {
            PickUp();
        }

        // Pokud je zbraň vybavena a hráč stiskne klávesu Q, provedeme odhození zbraně
        if (isEquipped && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    private void PickUp()
    {
        isEquipped = true; // Zbraň je vybavena
        slotFull = true; // Slot pro zbraň je plný

        transform.SetParent (gunContainer); // Nastavení rodiče zbraně na gunContainer
        transform.localPosition = Vector3.zero; // Nulování pozice zbraně v rámci gunContainer
        transform.localRotation = Quaternion.Euler(Vector3.zero); // Nulování rotace zbraně
        transform.localScale = Vector3.one; // Nastavení jednotkového měřítka pro velikost zbraně

        rb.isKinematic = true; // Kinematické pohyby jsou povoleny
        coll.isTrigger = true; // Kolidování je vypnuto

        gunScript.enabled = true; // Zapnutí skriptu pro zbraň

        SetCullingMasks(true); // Nastavení masky pro vykreslování kamer
    }

    private void Drop()
    {
        isEquipped = false; // Zbraň není vybavena
        slotFull = false; // Slot pro zbraň je prázdný

        transform.SetParent(null); // Zrušení rodiče zbraně

        rb.isKinematic = false; // Nekinematické pohyby jsou povoleny
        coll.isTrigger = false; // Kolidování je povoleno

        rb.velocity = player.GetComponentInParent<Rigidbody>().velocity; // Nastavení rychlosti zbraně na rychlost hráče

        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse); // Aplikace síly na zbraň vpřed
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse); // Aplikace síly na zbraň nahoru
        float random = Random.Range(-1f, 1f); // Generování náhodného čísla v rozmezí <-1, 1>
        rb.AddTorque(new Vector3(random, random, random) * 10); // Aplikace točivé síly na zbraň

        gunScript.enabled = false; // Vypnutí skriptu pro zbraň

        SetCullingMasks(false); // Nastavení masky pro vykreslování kamer
    }

    private void SetCullingMasks(bool isEquipped)
    {
        if (isEquipped)
        {
            PlayerCam.cullingMask &= ~(1 << whatIsWeaponLayer); // Vypnutí vykreslování zbraně pro kameru hráče
            GunCam.cullingMask |= (1 << whatIsWeaponLayer); // Zapnutí vykreslování zbraně pro kameru zbraně
        }
        else
        {
            PlayerCam.cullingMask |= (1 << whatIsWeaponLayer); // Zapnutí vykreslování zbraně pro kameru hráče
            GunCam.cullingMask &= ~(1 << whatIsWeaponLayer); // Vypnutí vykreslování zbraně pro kameru zbraně
        }
    }
}
