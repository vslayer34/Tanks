using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        // instantiate the explosive particle first and disable it for later use
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }

    /// <summary>
    /// Set up the tanks starting condition on spawn
    /// </summary>
    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }
    

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        m_CurrentHealth -= amount;

        SetHealthUI();

        if (m_CurrentHealth <= 0.0f && !m_Dead)
            OnDeath();
    }


    /// <summary>
    /// Set the visual elements of ui slider
    /// </summary>
    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;

        // linear transit between 0(zero health color) and 1(full health color)
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

    /// <summary>
    /// Play the effects for the death of the tank and deactivate it.
    /// </summary>
    private void OnDeath()
    {
        // transform the explosion particle to the tank transform then enable it
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        // play the particle and its sound
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        // Disable the tank
        gameObject.SetActive(false);
    }
}