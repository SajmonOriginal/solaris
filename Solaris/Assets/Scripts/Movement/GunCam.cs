using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCam : MonoBehaviour
{
    public float weaponSwayAmount = 0.02f;
    public PickUpController pickUpController; // Add a reference to the PickUpController

    private Transform _gunTransform;
    private Vector3 _initialPosition;

    void Start()
    {
        _gunTransform = transform;
        _initialPosition = _gunTransform.localPosition;
    }

    void Update()
    {
        // Check if the weapon is equipped before moving it
        if (pickUpController != null && pickUpController.isEquipped)
        {
            float mouseX = Input.GetAxis("Mouse X") * weaponSwayAmount;
            float mouseY = Input.GetAxis("Mouse Y") * weaponSwayAmount;

            // Convert mouse movement to world space
            Vector3 worldMouseMovement = new Vector3(-mouseX, -mouseY, 0f);

            // Apply weapon sway
            Vector3 targetPosition = _initialPosition + worldMouseMovement;
            _gunTransform.localPosition = Vector3.Lerp(_gunTransform.localPosition, targetPosition, Time.deltaTime * 4f);
        }
    }
}
