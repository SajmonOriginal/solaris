using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    // Nastavení citlivosti pohybu myši na ose X a Y
    public float sensX;

    public float sensY;

    // Reference na objekt orientace hráče
    public Transform orientation;
    public Transform camHolder;

    // Proměnné pro rotaci kamery
    float xRotation;

    float yRotation;

    // Metoda Start se volá při inicializaci objektu
    private void Start()
    {
        // Uzamknutí kurzoru na střed obrazovky a jeho skrytí
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Metoda Update se volá každý snímek
    private void Update()
    {
        // Získání vstupu myši
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // Přidávání rotace na ose Y v závislosti na pohybu myši
        yRotation += mouseX;

        // Přidávání rotace na ose X v závislosti na pohybu myši a její omezení na rozsah -90 až 90 stupňů
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Nastavení rotace kamery a orientace hráče
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

public void DoTilt(float zTilt)
{
    transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f, RotateMode.Fast);
}
}
