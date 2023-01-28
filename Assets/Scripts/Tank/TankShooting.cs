using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;

    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;                


    private void OnEnable()
    {
        // set up the launch parameters when the tank is first spawned
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        // The rate that the launch force charges up
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    /// <summary>
    /// Track the current state of the fire button and make decisions based on the current launch force.
    /// </summary>
    private void Update()
    {
        // always set the value to minimum unless the player start firing
        m_AimSlider.value = m_MinLaunchForce;

        // if the player hold the fire button too long it get to the max launch force then fire the shell
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        // first time the player clicked the fire button prevent the fire untill relase and set the launch force to minimum
        else if (Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;
        }
        // increase the launch force with according to the time the button is held and adjust the slider to show that
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;

            // set the audio source to the charging clip and play it
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        // launch the shell
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            Fire();
        }
    }


    /// <summary>
    /// Instantiate and launch the shell.
    /// </summary>
    private void Fire()
    {
        m_Fired = true;

        Rigidbody shellRigidBody = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // set the shell velocity acorrding to the launch force
        shellRigidBody.velocity = transform.forward * m_CurrentLaunchForce;

        // change the audio clip to the fireing audio and play it
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // reset the launch force to the minimum again
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}