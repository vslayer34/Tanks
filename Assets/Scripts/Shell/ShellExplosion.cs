using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                                // mask for the tanks objects
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                            // The max damage that can be don
    public float m_ExplosionForce = 1000f;                      
    public float m_MaxLifeTime = 2f;                            // life time for the shell
    public float m_ExplosionRadius = 5f;                        


    private void Start()
    {
        // initially set the shell to self destruct if it hit nothing
        Destroy(gameObject, m_MaxLifeTime);
    }

    /// <summary>
    /// Find all the tanks in an area around the shell and damage them.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // make an array of all tanks in the shell explosion radius using the tank mask
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            // access the rigid body of the tank
            Rigidbody targetRigidBody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidBody)
                continue;

            // add eplosive force to the tank
            targetRigidBody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            // access the health script of the tank
            TankHealth targetHealth = targetRigidBody.GetComponent<TankHealth>();

            if (!targetHealth)
                continue;

            // calculate and apply the damage
            float damage = CalculateDamage(targetRigidBody.position);
            targetHealth.TakeDamage(damage);
        }
        // remove the particles of the shell game object as a child so it don't get destroy with it
        m_ExplosionParticles.transform.parent = null;
        
        // play the particle and the explosion sound
        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        // destroy the particle after it finished playing
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);

        Destroy(gameObject);
    }

    /// <summary>
    /// Calculate the amount of damage a target should take based on it's position.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    private float CalculateDamage(Vector3 targetPosition)
    {
        // create a vector from the shell explosion position to the target
        Vector3 explosionToTargat = targetPosition - transform.position;

        // calculate the distance to the target
        float distanceToTarget = explosionToTargat.magnitude;

        // calculate the relative distance so it can applied as a percentage to the damage
        float relativeDistance = (m_ExplosionRadius - distanceToTarget) / m_ExplosionRadius;

        // apply the damage relatively to the distance
        float damage = relativeDistance * m_MaxDamage;

        // prevent the damage of ever being in negatives
        return Mathf.Max(0.0f, damage);
    }
}