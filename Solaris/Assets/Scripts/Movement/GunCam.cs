using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCam : MonoBehaviour
{
    public float weaponSwayAmount = 0.02f;

    private Transform _gunTransform;
    private Vector3 _initialPosition;

    void Start()
    {
        _gunTransform = transform;
        _initialPosition = _gunTransform.localPosition;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 targetPosition = new Vector3(
            _initialPosition.x - mouseX * weaponSwayAmount,
            _initialPosition.y - mouseY * weaponSwayAmount,
            _initialPosition.z
        );

        _gunTransform.localPosition = Vector3.Lerp(_gunTransform.localPosition, targetPosition, Time.deltaTime * 4f);
    }
}