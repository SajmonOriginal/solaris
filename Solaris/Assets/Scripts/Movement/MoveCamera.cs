using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition; // Cílová pozice kamery

    private void Update()
    {
        transform.position = cameraPosition.position; // Aktualizace pozice kamery na pozici cílového bodu
    }
}
