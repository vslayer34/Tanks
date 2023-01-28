using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                         // smooth the camera movement
    public float m_ScreenEdgeBuffer = 4f;                   // releive space for the camera
    public float m_MinSize = 6.5f;                          // Minimum size so the camera doesn't zoom in too much
    [HideInInspector] public Transform[] m_Targets; 


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                              // for smoothing camera movement
    private Vector3 m_MoveVelocity;                         // for smoothing the camera zooming
    private Vector3 m_DesiredPosition;                      // camera new position


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    /// <summary>
    /// Get the new camera position and smoothly move there
    /// </summary>
    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    /// <summary>
    /// find the average position of all tanks so that would be the new camera position to show all tanks in the scene
    /// </summary>
    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        // calculate the average vector of the tanks
        if (numTargets > 0)
            averagePos /= numTargets;

        // make sure the camera rig position stays at zero
        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    /// <summary>
    /// Find the new zoom level to show all the tanks and smoothly tranit to it
    /// </summary>
    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    /// <summary>
    /// Campare the position of the tanks to the camera rig
    /// so the max distance to the center either in axis X or Y is the new size as be dafault it would show all tanks
    /// add space relieve for the camera
    /// </summary>
    /// <returns>
    /// new size of the camera
    /// </returns>
    private float FindRequiredSize()
    {
        // get the local position of the camera rig in the orth mode
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            // get the tank position in the orth camer mode
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            // get the distance between the tank and the current pos
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;


            // get which size is bigger current or size of pos X
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));


            // get which size is bigger between (current or size of pos X) or pos Y
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        // apply the relive space for the new size
        size += m_ScreenEdgeBuffer;

        // make sure the size isn't smaller than the minimum size
        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    /// <summary>
    /// snap to original position and size of the Camera each new round
    /// </summary>
    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}