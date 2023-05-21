using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class DragRigidbody : MonoBehaviour
{
    public float force = 600;

    public float damping = 6;

    public float distance = 4;

    public LineRenderer lr;

    public Transform lineRenderLocation;

    private Transform jointTrans;

    private float dragDepth;

    public static bool isDragging = false;

    // Inicializace LineRendereru
    void Start()
    {
        lr.positionCount = 0;
    }

    // Po kliknutí tlačítkem myši
    void OnMouseDown()
    {
        HandleInputBegin(Input.mousePosition);
    }

    // Po uvolnění tlačítka myši
    void OnMouseUp()
    {
        HandleInputEnd(Input.mousePosition);
    }

    // Při pohybu myší
    void OnMouseDrag()
    {
        if (isDragging)
        {
            HandleInput(Input.mousePosition);
        }
    }

    // Zpracování začátku vstupu (kliknutí myší)
    public void HandleInputBegin(Vector3 screenPosition)
    {
        var ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        // Raycast pro zjištění kolize s objektem
        if (Physics.Raycast(ray, out hit, distance))
        {
            // Kontrola, zda kolize proběhla s objektem na vrstvě "Interactive"
            if (
                hit.transform.gameObject.layer ==
                LayerMask.NameToLayer("Interactive")
            )
            {
                dragDepth =
                    CameraPlane.CameraToPointDepth(Camera.main, hit.point);
                jointTrans = AttachJoint(hit.rigidbody, hit.point);
                isDragging = true;
            }
        }
        lr.positionCount = 2; // Nastavení počtu bodů pro LineRenderer
    }

    // Zpracování pohybu myší
    public void HandleInput(Vector3 screenPosition)
    {
        if (jointTrans == null) return;
        var worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        jointTrans.position =
            CameraPlane
                .ScreenToWorldPlanePoint(Camera.main,
                dragDepth,
                screenPosition);
        DrawRope(); // Vykreslení lana pomocí LineRendereru
    }

    // Zpracování konce vstupu (uvolnění myši)
    public void HandleInputEnd(Vector3 screenPosition)
    {
        DestroyRope(); // Zničení lana (LineRendereru)
        if (jointTrans != null)
        {
            Destroy(jointTrans.gameObject); // Zničení připojeného objektu
            jointTrans = null;
        }
        isDragging = false;
    }

    // Připojení spojovacího objektu k cílovému objektu
    private Transform AttachJoint(Rigidbody rb, Vector3 attachmentPosition)
    {
        GameObject go = new GameObject("Attachment Point");
        go.hideFlags = HideFlags.HideInHierarchy;
        go.transform.position = attachmentPosition;

        var newRb = go.AddComponent<Rigidbody>();
        newRb.isKinematic = true;

        var joint = go.AddComponent<ConfigurableJoint>();
        joint.connectedBody = rb;
        joint.configuredInWorldSpace = true;
        joint.xDrive = NewJointDrive(force, damping);
        joint.yDrive = NewJointDrive(force, damping);
        joint.zDrive = NewJointDrive(force, damping);
        joint.slerpDrive = NewJointDrive(force, damping);
        joint.rotationDriveMode = RotationDriveMode.Slerp;

        return go.transform;
    }

    // Vytvoření nového spojového pohonu pro konfigurovatelný kloub
    private JointDrive NewJointDrive(float force, float damping)
    {
        JointDrive drive = new JointDrive();
        drive.positionSpring = force;
        drive.positionDamper = damping;
        drive.maximumForce = Mathf.Infinity;
        return drive;
    }

    // Vykreslení lana pomocí LineRendereru
    private void DrawRope()
    {
        if (jointTrans == null)
        {
            return;
        }

        Camera cam = lineRenderLocation.parent.GetComponentInChildren<Camera>();
        Vector3 screenCenter =
            new Vector3(Screen.width / 2,
                Screen.height / 2,
                Vector3
                    .Distance(cam.transform.position, this.transform.position));
        Vector3 worldCenter = cam.ScreenToWorldPoint(screenCenter);

        lr.SetPosition(0, worldCenter);
        lr.SetPosition(1, this.transform.position);
    }

    // Zničení lana (LineRendereru)
    private void DestroyRope()
    {
        lr.positionCount = 0;
    }
}
