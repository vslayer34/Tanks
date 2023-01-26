using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;  


    private Quaternion m_RelativeRotation;     


    private void Start()
    {
        // Get the rotation value of the parent canvas
        m_RelativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
        // reapply the start rotation value every frame
        // so the health wheel doesn't rotate indebented from the tank
        // Keeping it fixed in place
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;
    }
}
