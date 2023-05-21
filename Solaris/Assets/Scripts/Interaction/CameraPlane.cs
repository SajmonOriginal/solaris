using UnityEngine;

public static class CameraPlane
{
    // Převádí souřadnice ve viewportu na světové souřadnice roviny s danou hloubkou z hlediska kamery.
    public static Vector3
    ViewportToWorldPlanePoint(
        Camera theCamera,
        float zDepth,
        Vector2 viewportCoord
    )
    {
        Vector2 angles = ViewportPointToAngle(theCamera, viewportCoord);
        float xOffset = Mathf.Tan(angles.x) * zDepth;
        float yOffset = Mathf.Tan(angles.y) * zDepth;
        Vector3 cameraPlanePosition = new Vector3(xOffset, yOffset, zDepth);
        cameraPlanePosition =
            theCamera.transform.TransformPoint(cameraPlanePosition);
        return cameraPlanePosition;
    }

    // Převádí souřadnice na obrazovce na světové souřadnice roviny s danou hloubkou z hlediska kamery.
    public static Vector3
    ScreenToWorldPlanePoint(Camera camera, float zDepth, Vector3 screenCoord)
    {
        var point = Camera.main.ScreenToViewportPoint(screenCoord);
        return ViewportToWorldPlanePoint(camera, zDepth, point);
    }

    // Převádí bod ve viewportu na úhly X a Y frustumu vzhledem k dané kameře.
    public static Vector2
    ViewportPointToAngle(Camera cam, Vector2 viewportCoord)
    {
        float adjustedAngle =
            AngleProportion(cam.fieldOfView / 2, cam.aspect) * 2;
        float xProportion = ((viewportCoord.x - .5f) / .5f);
        float yProportion = ((viewportCoord.y - .5f) / .5f);
        float xAngle =
            AngleProportion(adjustedAngle / 2, xProportion) * Mathf.Deg2Rad;
        float yAngle =
            AngleProportion(cam.fieldOfView / 2, yProportion) * Mathf.Deg2Rad;
        return new Vector2(xAngle, yAngle);
    }

    // Vrací vzdálenost mezi kamerou a rovinou rovnoběžnou s viewportem, která prochází daným bodem.
    public static float CameraToPointDepth(Camera cam, Vector3 point)
    {
        Vector3 localPosition = cam.transform.InverseTransformPoint(point);
        return localPosition.z;
    }

    // Vypočítá úhel na základě poměru úhlu a zlomku.
    public static float AngleProportion(float angle, float proportion)
    {
        float opposite = Mathf.Tan(angle * Mathf.Deg2Rad);
        float oppositeProportion = opposite * proportion;
        return Mathf.Atan(oppositeProportion) * Mathf.Rad2Deg;
    }
}
