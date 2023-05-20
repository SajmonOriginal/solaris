using UnityEngine;

public class DetectWeapons : MonoBehaviour
{
    private PickUpController pickedUpWeapon;

    private void OnTriggerEnter(Collider other)
    {
        var weapon = other.GetComponent<PickUpController>();
        if (weapon != null)
        {
            pickedUpWeapon = weapon;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var weapon = other.GetComponent<PickUpController>();
        if (weapon != null && pickedUpWeapon == weapon)
        {
            pickedUpWeapon = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && pickedUpWeapon != null)
        {
            pickedUpWeapon.PickUp();
        }
    }
}
