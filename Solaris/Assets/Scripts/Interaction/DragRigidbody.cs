using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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

    void Start()
    {
        lr.positionCount = 0;
    }

    void OnMouseDown()
    {
        HandleInputBegin(Input.mousePosition);
    }

    void OnMouseUp()
    {
        HandleInputEnd(Input.mousePosition);
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            HandleInput(Input.mousePosition);
        }
    }

    public void HandleInputBegin(Vector3 screenPosition)
    {
        var ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactive"))
            {
                dragDepth = CameraPlane.CameraToPointDepth(Camera.main, hit.point);
                jointTrans = AttachJoint(hit.rigidbody, hit.point);
                isDragging = true;
            }
        }
        lr.positionCount = 2;
    }

    public void HandleInput(Vector3 screenPosition)
    {
        if (jointTrans == null) return;
        var worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        jointTrans.position = CameraPlane.ScreenToWorldPlanePoint(Camera.main, dragDepth, screenPosition);
        DrawRope();
    }

    public void HandleInputEnd(Vector3 screenPosition)
    {
        DestroyRope();
        if(jointTrans != null)
        {
            Destroy(jointTrans.gameObject);
            jointTrans = null;
        }
        isDragging = false;
    }

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

    private JointDrive NewJointDrive(float force, float damping)
    {
        JointDrive drive = new JointDrive();
        drive.positionSpring = force;
        drive.positionDamper = damping;
        drive.maximumForce = Mathf.Infinity;
        return drive;
    }

    private void DrawRope()
    {
        if (jointTrans == null)
        {
            return;
        }

        Camera cam = lineRenderLocation.parent.GetComponentInChildren<Camera>();
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, Vector3.Distance(cam.transform.position, this.transform.position));
        Vector3 worldCenter = cam.ScreenToWorldPoint(screenCenter);

        lr.SetPosition(0, worldCenter);
        lr.SetPosition(1, this.transform.position);
    }

    private void DestroyRope()
    {
        lr.positionCount = 0;
    }
}