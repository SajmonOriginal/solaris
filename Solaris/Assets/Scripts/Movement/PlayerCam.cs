using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    // Citlivost pohybu myši
    public float sensX;

    public float sensY;

    // Odkaz na orientaci hráče a na držák kamery
    public Transform orientation;

    public Transform camHolder;

    // Proměnné pro rotaci kamery
    float xRotation;

    float yRotation;

    private void Start()
    {
        // Zamknutí kurzoru myši a skrytí kurzoru
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Získání vstupu pohybu myši
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // Rotace kolem osy Y
        yRotation += mouseX;

        // Rotace kolem osy X s omezením rozsahu
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotace kamery a orientace
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    // Animace změny zorného pole kamery
    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.40f);
    }

    // Animace naklonění kamery
    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.40f);
    }
}
